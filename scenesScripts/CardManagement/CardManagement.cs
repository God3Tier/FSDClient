using Godot;
using System;
using System.Threading.Tasks;
using System.Linq;
using FSDClient.autoLoad;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using FSDClient.card;
using FSDClient.resource;
	
public class GetCollectionResponse
{
	[JsonPropertyName("decks")]
	public List<Collection> Decks { get; set; }
}

public class Collection
{
	[JsonPropertyName("deck_id")]
	public int DeckId { get; set; }

	[JsonPropertyName("deck_name")]
	public string DeckName { get; set; }

	[JsonPropertyName("cards_not_in_deck")]
	public List<CardQuantity> CardsNotInDeck { get; set; }
}

public class CardQuantity
{
	[JsonPropertyName("card_id")]
	public int CardId { get; set; }
	
	[JsonPropertyName("level")]
	public int Level { get; set; }

	[JsonPropertyName("quantity")]
	public int Quantity { get; set; }
}

public class UpdateDeckRequest
{
	[JsonPropertyName("deck_id")]
	public int DeckId { get; set; }
	
	[JsonPropertyName("name")]
	public string Name { get; set; } = "";
	
	[JsonPropertyName("card_ids")]
	public List<int> CardIds { get; set; } = new();
}

public class GetActiveResponse
{
	[JsonPropertyName("active_deck_id")]
	public int ActiveDeckId { get; set; }
}

public class GetDecksResponse
{
	[JsonPropertyName("count")]
	public int Count { get; set; }
	[JsonPropertyName("decks")]
	public List<Deck> Decks { get; set; }
}


public class Deck
{
	[JsonPropertyName("deck_id")]
	public int DeckId { get; set; }
	
	[JsonPropertyName("name")]
	public string Name { get; set; } = "";
	
	[JsonPropertyName("card_ids")]
	public List<int> CardIds { get; set; } = new();
	
	[JsonPropertyName("cards")]
	public List<CardInfo> Cards { get; set; } = new();
	
	[JsonPropertyName("is_active")]
	public bool IsActive { get; set; } = false;
}

public class CardInfo
{
	[JsonPropertyName("card_id")]
	public int CardId { get; set; }
	
	[JsonPropertyName("position")]
	public int Position { get; set; }
	
	[JsonPropertyName("level")]
	public int Level { get; set; } = 1;

}

public class LevelUpCardResponse
{
	[JsonPropertyName("card_id")]
	public int CardId { get; set; }

	[JsonPropertyName("new_level")]
	public int NewLevel { get; set; }

	[JsonPropertyName("cards_consumed")]
	public int CardsConsumed { get; set; }

	[JsonPropertyName("crystals_spent")]
	public int CrystalsSpent { get; set; }

	[JsonPropertyName("crystals_left")]
	public int CrystalsLeft { get; set; }

	[JsonPropertyName("quantity_left")]
	public int QuantityLeft { get; set; }

	[JsonPropertyName("pruned_decks")]
	public List<PrunedDeck> PrunedDecks { get; set; } = new();
}

public class PrunedDeck
{
	[JsonPropertyName("deck_id")]
	public int DeckId { get; set; }

	[JsonPropertyName("deck_name")]
	public string DeckName { get; set; } = "";

	[JsonPropertyName("removed")]
	public int Removed { get; set; }
}
	
public partial class CardManagement : Control
{
	private PlayerStateManager CurrentPlayer { get; set; }
	private NetworkManager Network { get; set; }
	private PackedScene CardScene = GD.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");
	private GetDecksResponse Decks;
	private List<Collection> Collections;
	private int SelectedDeck = 0;
	private int InitialActiveDeckId = 0;
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Network = NetworkManager.Instance;
		try
		{
			CurrentPlayer = PlayerStateManager.Instance;
			GD.Print(CurrentPlayer.ToString());
			if (CurrentPlayer.PlayerData == null)
			{
				GD.Print("The PlayerData is empty");
				var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
				GameStateManager.ChangeGameState(GameState.LOGIN);
				return;
			}

		}
		catch (Exception e)
		{
			GD.PrintErr("Whoops ", e);
		}
		
		InitialisePlayerInformation();
		FetchCollections();
		FetchDecks();
	}
	
		private void InitialisePlayerInformation()
	{
		// Set player Icon TODO: I need Windy's code to be merged before I can do this

		var Header = (BoxContainer)FindChild("Header");
		var Row1 = (BoxContainer)Header.FindChild("Row 1");

		// Set Header -> Row1 -> Crystal ->  Label
		var Cry = (Label)((ColorRect)Row1.FindChild("Crystal", true)).FindChild("Label");
		Cry.Text = CurrentPlayer.Crystal.ToString();


		// Set Header -> Row2 -> Name ->  Label
		var Username = (Label)((ColorRect)Row1.FindChild("Name", true)).FindChild("Label");
		Username.Text = CurrentPlayer.PlayerData.Username;


	}

	// Function to update crystal count in header and in player data
	private void UpdateCrystals(int CrystalsTotal)
	{
		var Header = (BoxContainer)FindChild("Header");
		var Row1 = (BoxContainer)Header.FindChild("Row 1");
		
		CurrentPlayer.Crystal = CrystalsTotal;

		// Set Header -> Row1 -> Crystal ->  Label
		var Cry = (Label)((ColorRect)Row1.FindChild("Crystal", true)).FindChild("Label");
		Cry.Text = CurrentPlayer.Crystal.ToString();

	}
	
	// Function to fetch collections from backend
	private void FetchCollections(){
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.DECK + "/players/me/cards/available", Godot.HttpClient.Method.Get, "", GetCollectionResponse);
	}
	
	private void GetCollectionResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);
		GD.Print("Collection");
		GD.Print(json);
		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Failed to get a successful response when fetching collections");
			GD.PrintErr(json);
			return;
		}


		var data = JsonSerializer.Deserialize<GetCollectionResponse>(json);
		Collections = data.Decks;
		GenerateCollection();
	}

	// Function to generate card collection
	public void GenerateCollection()
	{
		// Generate collections
		for (int j = 0; j < Collections.Count; j++)
		{
			int CollectionIndex = j;
			var CollectionContainer = new HFlowContainer();
			CollectionContainer.CustomMinimumSize = new Vector2(1000, 650);
			CollectionContainer.Name = "CollectionContainer";
			
			// Generate collection cards
			for (int i = 0; i < Collections[j].CardsNotInDeck.Count; i++)
			{
				// to pass the values over to any functions (passing i will always pass max)
				int CardIndex = i;
				
				var CardContainer =  CreateCardWithLabel("Collection", Collections[CollectionIndex].CardsNotInDeck[CardIndex].CardId, Collections[j].CardsNotInDeck[i].Quantity, Collections[j].CardsNotInDeck[i].Level);
				
				// Add the card into the main container
				CollectionContainer.AddChild(CardContainer);
			}
			
			// First Collection for deck 1 will be visible, rest won't
			if (j != 0){
				CollectionContainer.Visible = false;
			}
			
			// load it all in
			ScrollContainer CollectionsScrollContainer = GetNode<ScrollContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer");
			CollectionsScrollContainer.AddChild(CollectionContainer);
			
			// the button container must always be below in the tree so that its button will be overlayed on top
			Control ButtonContainer = CollectionsScrollContainer.GetNode<Control>("ButtonContainer");
			CollectionsScrollContainer.MoveChild(ButtonContainer, CollectionsScrollContainer.GetChildCount() - 1);
		}	
	}
	
	// When a Card is being pressed
	private void OnCardPressed(string Location, int CardId, Vector2 pos){
		Control ButtonContainer;
		Panel AllCardsContainer;
		if(Location == "Collection"){
			AllCardsContainer = GetNode<Panel>("MainCardContainer/CollectionContainer/CollectionColorContainer");
		}else{
			AllCardsContainer = GetNode<Panel>("MainCardContainer/DeckContainer/DeckColorContainer");
		}
		
		
		// Find the cards being displayed (as decks have multiple but only 1 displayed)
		ScrollContainer Scroll = AllCardsContainer.GetNode<ScrollContainer>("ScrollContainer");
		FlowContainer CardFlowContainer = Scroll.GetChild(SelectedDeck) as HFlowContainer;
		VBoxContainer CardContainer = null;
		

		ButtonContainer = Scroll.GetNode<Control>("ButtonContainer");	
		
		// Check if card button exists
		int ButtonIndex = CheckCardButton(pos.X, pos.Y, ButtonContainer);
		// Button does not exist yet
		if(ButtonIndex == -1){
			// Create button
			CreateCardButton(pos.X, pos.Y, CardId, Location, ButtonContainer);
			RefreshScrollbar(AllCardsContainer);
		}else{
			// Else deletes it
			DeleteCardButton(ButtonIndex, ButtonContainer);
		}
	}
	
	// Need to refresh because the extra card buttons won't be included in the initial calculations
	private void RefreshScrollbar(Panel AllCardsContainer){
		ScrollContainer Scroll = AllCardsContainer.GetNode<ScrollContainer>("ScrollContainer");
		HFlowContainer Content = null;
		
		// Find the cards being displayed (as decks have multiple but only 1 displayed)
		Content = Scroll.GetChild(SelectedDeck) as HFlowContainer;
			

		// Calculate total Content height
		float TotalHeight = 0;
		foreach(Control Child in Content.GetChildren().OfType<Control>())
		{
			Vector2 pos = Child.Position;
			if(TotalHeight < pos.Y){
				TotalHeight = pos.Y;
			}
		}

		float DesiredHeight = TotalHeight + 300;
		Content.CustomMinimumSize = new Vector2(Scroll.Size.X, DesiredHeight);
	}
	
	// Function to check if card button set exists, return the index of the element child
	private int CheckCardButton(float X, float Y, Control ButtonContainer){
		
		for (int i = 0; i < ButtonContainer.GetChildCount(); i++){
			Control ButtonSet = ButtonContainer.GetChild(i) as Control;
			// use position to find the first button
			if(ButtonSet.Position == new Vector2(X + 10, Y + 200)){
				return i;
			}
		}
		return -1;
	}
	
	private void DeleteCardButton(int index, Control ButtonContainer){		
		Control ButtonSet = ButtonContainer.GetChild(index) as Control;
		ButtonSet.QueueFree();
	}
	
	// Function to create the 2 buttons
	private void CreateCardButton(float X, float Y, int CardId, string Location, Control ButtonContainer){
		var ButtonSet = new Control();
		ButtonSet.Position = new Vector2(X + 10, Y + 200);
		
		// Add the 2 button (more info & add to deck)
		var InfoButton = new Button();
		var AddOrRemoveDeckButton = new Button();
		
		InfoButton.CustomMinimumSize = new Vector2(170, 40);
		InfoButton.Position = new Vector2(0, 0);
		InfoButton.Text = "Info";
		InfoButton.ZIndex = 3;
		InfoButton.AddThemeFontSizeOverride("font_size", 20);
		InfoButton.AddThemeColorOverride("font_color", new Color(0, 0, 0));
		InfoButton.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
			
		AddOrRemoveDeckButton.CustomMinimumSize = new Vector2(170, 40);
		AddOrRemoveDeckButton.Position = new Vector2(0, 40);
		if(Location == "Collection"){
			AddOrRemoveDeckButton.Text = "Add";
		}else if(Location == "Deck"){
			AddOrRemoveDeckButton.Text = "Remove";
		}
		AddOrRemoveDeckButton.ZIndex = 3;
		AddOrRemoveDeckButton.AddThemeFontSizeOverride("font_size", 20);
		AddOrRemoveDeckButton.AddThemeColorOverride("font_color", new Color(0, 0, 0));
		AddOrRemoveDeckButton.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
		
		StyleBox button = GD.Load<StyleBox>("res://styles/button.tres");
		StyleBox ButtonHover = GD.Load<StyleBox>("res://styles/button.tres");
		StyleBox ButtonPressed = GD.Load<StyleBox>("res://styles/button.tres");

		InfoButton.AddThemeStyleboxOverride("normal", button);
		InfoButton.AddThemeStyleboxOverride("hover", ButtonHover);
		InfoButton.AddThemeStyleboxOverride("pressed", ButtonPressed);
			
		AddOrRemoveDeckButton.AddThemeStyleboxOverride("normal", button);
		AddOrRemoveDeckButton.AddThemeStyleboxOverride("hover", ButtonHover);
		AddOrRemoveDeckButton.AddThemeStyleboxOverride("pressed", ButtonPressed);
			
		// Connect pressed to a function
		InfoButton.Pressed += () => OpenCardPopup(CardId);
		
		if(Location == "Collection"){
			AddOrRemoveDeckButton.Pressed += () => AddIntoDeck(CardId);
		}else{
			AddOrRemoveDeckButton.Pressed += () => RemoveFromDeck(CardId, X, Y);
		}
		
		ButtonSet.AddChild(InfoButton);
		ButtonSet.AddChild(AddOrRemoveDeckButton);
	
		ButtonContainer.AddChild(ButtonSet);
	}
	
	// Function to fetch deck from backend 
	private void FetchDecks(){
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.DECK + "/decks", Godot.HttpClient.Method.Get, "", GetDecksResponse);
	}
	
	// Function to fetch active deck from backend
	private void FetchActive(){
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.DECK + "/decks/active", Godot.HttpClient.Method.Get, "", GetActiveResponse);
	}
	
	// response handler for get all decks	
	private void GetDecksResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);
		GD.Print("deck");
		GD.Print(json);
		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Failed to get a successful response when fetching decks");
			GD.PrintErr(json);
			return;
		}


		Decks = JsonSerializer.Deserialize<GetDecksResponse>(json);
		FetchActive();
	}
	
	//	response handler for get active deck
	private void GetActiveResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);
		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Failed to get a successful response when fetching active deck");
			GD.PrintErr(json);
			return;
		}


		InitialActiveDeckId = JsonSerializer.Deserialize<GetActiveResponse>(json).ActiveDeckId;
		
		// set active deck
		for(int i = 0; i < Decks.Count; i++){
			Decks.Decks[i].IsActive = (Decks.Decks[i].DeckId == InitialActiveDeckId);
		}
		GenerateDecks();
	}

	// Function to generate Decks
	private async Task GenerateDecks()
	{
		// Generate decks
		for (int j = 0; j < Decks.Decks.Count; j++)
		{
			int DeckIndex = j;
			// insert cards into deck
			var DeckContainer = new HFlowContainer();
			DeckContainer.CustomMinimumSize = new Vector2(610, 660);
			DeckContainer.Name = "DeckContainer";
			
			// Generate Cards
			for (int i = 0; i < Decks.Decks[j].Cards.Count; i++)
			{
				// to pass the values over to any functions (passing i will always pass max)
				int CardIndex = i;
				
				// Create control that gives the card its size
				var CardContainer = CreateCard("Deck", Decks.Decks[DeckIndex].Cards[CardIndex].CardId, Decks.Decks[DeckIndex].Cards[CardIndex].Level);
				
				// Add the card into the main container
				DeckContainer.AddChild(CardContainer);
			}
			
			
			// First deck will be visible, rest won't
			if (j != 0){
				DeckContainer.Visible = false;
			}
			
			// load it all in
			ScrollContainer DecksScrollContainer = GetNode<ScrollContainer>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer");
			DecksScrollContainer.AddChild(DeckContainer);
			
			// the button container must always be below in the tree so that its button will be overlayed on top
			Control ButtonContainer = GetNode<Control>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer/ButtonContainer");
			DecksScrollContainer.MoveChild(ButtonContainer, DecksScrollContainer.GetChildCount() - 1);
		}
		
		// Chain to tabs AFTER decks finish
		CallDeferred(MethodName.CreateDynamicTabs);
		
		UpdateActiveButton();
	}
	
	// Create Card with count (For collection)
	private VBoxContainer CreateCardWithLabel(string Location, int CardId, int Quantity, int Level)
	{
		var CardContainer = CreateCard(Location, CardId, Level);
		
		var CardCount = new Label();
		CardCount.Name = "CardCount";
		CardCount.Text = "X" + Quantity;
		CardCount.AddThemeFontSizeOverride("font_size", 23);
		CardCount.AddThemeColorOverride("font_color", new Color(0, 0, 0));
		CardCount.HorizontalAlignment = HorizontalAlignment.Center;
		
		// Add the card count label into the container
		CardContainer.AddChild(CardCount);
		
		return CardContainer;
	}
	
	// Create just Card
	private VBoxContainer CreateCard(string Location, int CardId, int Level)
	{
		// Create control that gives the card its size
		var CardContainer = new VBoxContainer();
		CardContainer.CustomMinimumSize = new Vector2(190, 200);
		CardContainer.Name = "" + CardId;
		
		// Create control that gives the card its size
		var CardControl = new Control();
		CardControl.CustomMinimumSize = new Vector2(190, 200);
		CardControl.Name = "CardControl";
				
		// Create the card itself
		var Card = CardBuilder.GenerateCardWithLevel(CardId, Level);
			
		Card.Position = new Vector2(95, 100);
		CardControl.AddChild(Card);
				
				
		// Add card button overlay
		var CardButtonOverlay = new Button();
		CardButtonOverlay.CustomMinimumSize = new Vector2(170, 200);
		CardButtonOverlay.Position = new Vector2(10, 0);
		CardButtonOverlay.ZIndex = 2;
		CardButtonOverlay.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
				
		// Make all states fully transparent
		var Transparent = new StyleBoxFlat();
		Transparent.BgColor = new Color(0, 0, 0, 0); // RGBA, alpha = 0 (transparent)

		CardButtonOverlay.AddThemeStyleboxOverride("normal", Transparent);
		CardButtonOverlay.AddThemeStyleboxOverride("hover", Transparent);
		CardButtonOverlay.AddThemeStyleboxOverride("pressed", Transparent);
		CardButtonOverlay.AddThemeStyleboxOverride("focus", Transparent);
		CardButtonOverlay.AddThemeStyleboxOverride("disabled", Transparent);

		CardButtonOverlay.Pressed += () => OnCardPressed(Location, CardId, CardContainer.Position);
		CardControl.AddChild(CardButtonOverlay);
			
		// Add the card into the container
		CardContainer.AddChild(CardControl);
		
		
		return CardContainer;
	}
	
	// function to add a card into deck (called by button)
	private void AddIntoDeck(int CardId)
	{
		int Level = 1;
		// Remove collection		
		ScrollContainer CollectionScrollContainer = GetNode<ScrollContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer");
		HFlowContainer CollectionContainer = CollectionScrollContainer.GetChild(SelectedDeck) as HFlowContainer;
		VBoxContainer CardContainer;
		for(int i = 0; i < CollectionContainer.GetChildCount(); i++){
			CardContainer = CollectionContainer.GetChild(i) as VBoxContainer;
			
			if(CardContainer.Name == "" + CardId){
				Level = Collections[SelectedDeck].CardsNotInDeck[i].Level;

				Collections[SelectedDeck].CardsNotInDeck[i].Quantity -= 1;
				// No more in collection
				if(Collections[SelectedDeck].CardsNotInDeck[i].Quantity <= 0){
					CardContainer.QueueFree();
					Collections[SelectedDeck].CardsNotInDeck.Remove(Collections[SelectedDeck].CardsNotInDeck[i]);
				}else{
				// just update count
					Label CardCount = CardContainer.GetNode<Label>("CardCount");
					CardCount.Text = "X" + Collections[SelectedDeck].CardsNotInDeck[i].Quantity;
				}

			}
		}
		
		// Add into deck data
		Decks.Decks[SelectedDeck].CardIds.Add(CardId);
		Decks.Decks[SelectedDeck].Cards.Add(new CardInfo {CardId = CardId, Position = Decks.Decks[SelectedDeck].Cards.Count, Level = Level});
		
		// Add visually
		ScrollContainer DeckScrollContainer = GetNode<ScrollContainer>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer");
		HFlowContainer DeckContainer = DeckScrollContainer.GetChild(SelectedDeck) as HFlowContainer;
		CardContainer = CreateCard("Deck", CardId, Level);
		DeckContainer.AddChild(CardContainer);
		
		// Make save button visible
		SetSaveButtonVisibility(true);
		
		// Reset buttons if not it will be out of position
		RemoveButtonContainers();
	}
	
	private void RemoveFromDeck(int CardId, float X, float Y){
		int Level = 1;

		// Remove card from deck
		ScrollContainer DeckScrollContainer = GetNode<ScrollContainer>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer");
		HFlowContainer DeckContainer = DeckScrollContainer.GetChild(SelectedDeck) as HFlowContainer;
		VBoxContainer CardContainer;
		for(int i = 0; i < DeckContainer.GetChildCount(); i++){
			CardContainer = DeckContainer.GetChild(i) as VBoxContainer;
			Vector2 pos = CardContainer.Position;
			if(pos.X == X && pos.Y == Y){
				// get the level of the card
				Level = Decks.Decks[SelectedDeck].Cards[i].Level;
				// remove from cardID
				Decks.Decks[SelectedDeck].CardIds.Remove(Decks.Decks[SelectedDeck].CardIds[i]);
				// remove from Cards
				Decks.Decks[SelectedDeck].Cards.Remove(Decks.Decks[SelectedDeck].Cards[i]);
				// queuefree
				CardContainer.QueueFree();
				
			}
		}
		
		
		// insert into collection
		ScrollContainer CollectionScrollContainer = GetNode<ScrollContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer");
		HFlowContainer CollectionContainer = CollectionScrollContainer.GetChild(SelectedDeck) as HFlowContainer;
		
		// To check if card is in collection
		var CardExist = false;
		for(int i = 0; i < CollectionContainer.GetChildCount(); i++){
			CardContainer = CollectionContainer.GetChild(i) as VBoxContainer;
			
			// If there is atleast 1 still in collection
			if(CardContainer.Name == "" + CardId){
				Collections[SelectedDeck].CardsNotInDeck[i].Quantity += 1;
				
				// Update count
				Label CardCount = CardContainer.GetNode<Label>("CardCount");
				CardCount.Text = "X" + Collections[SelectedDeck].CardsNotInDeck[i].Quantity;
				
				CardExist = true;
			}
		}
		
		// if card did not exist in collection
		if(!CardExist){
			// insert into collection data
			Collections[SelectedDeck].CardsNotInDeck.Add(new CardQuantity {CardId = CardId, Quantity = 1, Level = Level});
			
			// Insert visually
			CardContainer = CreateCardWithLabel("Collection", CardId, 1, Level);
			CollectionContainer.AddChild(CardContainer);
		}
		
		// Make save button visible
		SetSaveButtonVisibility(true);
		
		// Reset buttons if not it will be out of position
		RemoveButtonContainers();

	}

	// create dynamic amount of tabs based on deck count
	private void CreateDynamicTabs()
	{
	 	var DeckTab = GetNode<TabBar>("MainCardContainer/DeckContainer/TabRow/DeckTab");
		
		// Create tabs
		for(int i = 0; i < Decks.Count; i++){
			DeckTab.AddTab("Deck " + (i + 1));
		}
		
		DeckTab.TabChanged += OnTabChanged;
	}
	
	// handle changing of tabs for decks
	private void OnTabChanged(long tabIndex)
	{
		ScrollContainer DecksScrollContainer = GetNode<ScrollContainer>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer");
		ScrollContainer CollectionsScrollContainer = GetNode<ScrollContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer");
		// Show/hide content based on tab
		for(int i = 0; i < Decks.Count; i++)
		{
			HFlowContainer CollectionContainer = CollectionsScrollContainer.GetChild(i) as HFlowContainer;
			HFlowContainer DeckContainer = DecksScrollContainer.GetChild(i) as HFlowContainer;
			// make to visible if its selected
			CollectionContainer.Visible = (i == tabIndex);
			DeckContainer.Visible = (i == tabIndex);

		}
		
		// Delete all past Card Buttons ("Info, "Remove") as there should not be any in new decks
		RemoveButtonContainers();
		
		// Update the main variable
		SelectedDeck = (int) tabIndex;
		
		// Change Active button
		UpdateActiveButton();
	}

	// function used to update active button visuals to be active/Set active
	private void UpdateActiveButton()
	{
		Button IsActiveButton = GetNode<Button>("MainCardContainer/DeckContainer/TabRow/IsActiveButton");

		if(Decks.Decks[SelectedDeck].IsActive){
			StyleBox ButtonSuccess = GD.Load<StyleBox>("res://styles/button_success.tres");
			StyleBox ButtonSuccessHover = GD.Load<StyleBox>("res://styles/button_success_hover.tres");
			StyleBox ButtonSuccessPressed = GD.Load<StyleBox>("res://styles/button_success_pressed.tres");

			IsActiveButton.AddThemeStyleboxOverride("normal", ButtonSuccess);
			IsActiveButton.AddThemeStyleboxOverride("hover", ButtonSuccessHover);
			IsActiveButton.AddThemeStyleboxOverride("pressed", ButtonSuccessPressed);
			IsActiveButton.Text = "Active";
		}else{
			StyleBox ButtonDanger = GD.Load<StyleBox>("res://styles/button_danger.tres");
			StyleBox ButtonDangerHover = GD.Load<StyleBox>("res://styles/button_danger_hover.tres");
			StyleBox ButtonDangerPressed = GD.Load<StyleBox>("res://styles/button_danger_pressed.tres");

			IsActiveButton.AddThemeStyleboxOverride("normal", ButtonDanger);
			IsActiveButton.AddThemeStyleboxOverride("hover", ButtonDangerHover);
			IsActiveButton.AddThemeStyleboxOverride("pressed", ButtonDangerPressed);
			IsActiveButton.Text = "Set Active";
		}	
		
	}

	// Function to delete  all past Card Buttons ("Info, "Remove") as there should not be any
	public void RemoveButtonContainers()
	{
		ScrollContainer DecksScrollContainer = GetNode<ScrollContainer>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer");
		ScrollContainer CollectionsScrollContainer = GetNode<ScrollContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer");
		Control DeckButtonContainer = DecksScrollContainer.GetNode<Control>("ButtonContainer");
		Control CollectionButtonContainer = CollectionsScrollContainer.GetNode<Control>("ButtonContainer");
		
		for(int i = 0; i < DeckButtonContainer.GetChildCount(); i++){
			DeckButtonContainer.GetChild(i).QueueFree();
		}
		
		for(int i = 0; i < CollectionButtonContainer.GetChildCount(); i++){
			CollectionButtonContainer.GetChild(i).QueueFree();
		}
	}

	public void _OnBackButtonPressed()
	{
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.HOMESCREEN);
	}
	
	public void _OnCardPopupBackgroundPressed()
	{
		CloseCardPopup();
	}	
	
	public void _OnCloseCardButtonPressed()
	{
		CloseCardPopup();
	}	
	
	private void OpenCardPopup(int CardId, int Level = 0, int Quantity = 0)
	{
		if(Level == 0){
			for (int i = 0; i < Collections[SelectedDeck].CardsNotInDeck.Count; i++){
				if(Collections[SelectedDeck].CardsNotInDeck[i].CardId == CardId){
					Level = Collections[SelectedDeck].CardsNotInDeck[i].Level;
					Quantity = Collections[SelectedDeck].CardsNotInDeck[i].Quantity;
				}
			}
			for (int i = 0; i < Decks.Decks[SelectedDeck].Cards.Count; i++){
				if(Decks.Decks[SelectedDeck].Cards[i].CardId == CardId){
					Level = Decks.Decks[SelectedDeck].Cards[i].Level;
					Quantity++;
				}
			}
		}
		
		Control CardPopupContainerNode = GetNode<Control>("CardPopupContainer");
		
		// Update the values
		CardStats CardValues = CardBuilder.GetCardValues(CardId);

		Panel CardPopupBox = CardPopupContainerNode.GetNode<Panel>("CardPopupBox");
		
		// Header card name
		Label Header = CardPopupBox.GetNode<Label>("Header");
		Header.Text = CardValues.Name;
		
		// Image
		TextureRect CardAppearance = CardPopupBox.GetNode<TextureRect>("CardDetails/LeftCardDetails/CardAppearance");
		CardAppearance.Texture = CardValues.Image;

		// Rarity
		Label RarityTextLabel = CardPopupBox.GetNode<Label>("CardDetails/LeftCardDetails/CardDescription/RarityContainer/RarityTextLabel");
		RarityTextLabel.Text = CardValues.Rarity;
		
		// Cost
		Label LevelLabel = CardPopupBox.GetNode<Label>("CardDetails/RightCardDetails/LevelLabel");
		LevelLabel.Text = "Level " + Level;

		// Cost
		Label CostTextLabel = CardPopupBox.GetNode<Label>("CardDetails/RightCardDetails/CardStats/CostContainer/CostLevelContainer/CostTextLabel");
		CostTextLabel.Text = CardValues.Cost.ToString();
		
		// Power
		Label PowerTextLabel = CardPopupBox.GetNode<Label>("CardDetails/RightCardDetails/CardStats/PowerContainer/PowerLevelContainer/PowerTextLabel");
		PowerTextLabel.Text = CardBuilder.LevelStatsCalculator(CardValues.Attack, Level).ToString();
		// Power level
		Label PowerLevelLabel = CardPopupBox.GetNode<Label>("CardDetails/RightCardDetails/CardStats/PowerContainer/PowerLevelContainer/PowerLevelLabel");
		PowerLevelLabel.Text = "+" + (CardBuilder.LevelStatsCalculator(CardValues.Attack, Level + 1) -  CardBuilder.LevelStatsCalculator(CardValues.Attack, Level)).ToString();

		// HP
		Label HPTextLabel = CardPopupBox.GetNode<Label>("CardDetails/RightCardDetails/CardStats/HPContainer/HPLevelContainer/HPTextLabel");
		HPTextLabel.Text = CardBuilder.LevelStatsCalculator(CardValues.Health, Level).ToString();
		// HP level
		Label HPLevelLabel = CardPopupBox.GetNode<Label>("CardDetails/RightCardDetails/CardStats/HPContainer/HPLevelContainer/HPLevelLabel");
		HPLevelLabel.Text = "+" + (CardBuilder.LevelStatsCalculator(CardValues.Health, Level + 1) -  CardBuilder.LevelStatsCalculator(CardValues.Health, Level)).ToString();

		// Effect
		Label AbilityTextLabel = CardPopupBox.GetNode<Label>("CardDetails/RightCardDetails/AbilityContainer/AbilityTextLabel");
		AbilityTextLabel.Text = CardValues.Effect;
		
		// Level
		ProgressBar LevelProgressBar = CardPopupBox.GetNode<ProgressBar>("CardDetails/RightCardDetails/LevelContainer/LevelProgressBar");
		int CardCost = CardBuilder.CardCostCalculator(Level);
		LevelProgressBar.Value = (float)Quantity / CardCost;
		
		Label ProgressLabel = CardPopupBox.GetNode<Label>("CardDetails/RightCardDetails/LevelContainer/LevelProgressBar/ProgressLabel");
		ProgressLabel.Text = Quantity + " / " + CardCost + " Copies";
		
		Button LevelButton = CardPopupBox.GetNode<Button>("CardDetails/RightCardDetails/LevelContainer/LevelButton");
		int CrystalCost = CardBuilder.CrystalCostCalculator(Level);
		
		// if not enough cards/crystals
		if(CurrentPlayer.Crystal < CrystalCost || Quantity < CardCost){
			// disable button and change text
			LevelButton.Text = "Can't Level Up...\nCost:" + CrystalCost + " Crystals";
			LevelButton.Disabled = true;
		}else{
			LevelButton.Text = "Level Up!\nCost:" + CrystalCost + " Crystals";
			LevelButton.Disabled = false;
			LevelButton.SetMeta("CardId", CardId);
			LevelButton.SetMeta("Level", Level);
			LevelButton.SetMeta("Quantity", Quantity);
		}

		// Turn it On (make visible true)
		CardPopupContainerNode.Visible = true;
	}	
	
	public void CloseCardPopup()
	{
		Control CardPopupContainerNode = GetNode<Control>("CardPopupContainer");
		
		// Turn it OFF (make visible false)
		CardPopupContainerNode.Visible = false;
	}	
	
	// When pressing save button
	private void _OnSaveButtonPressed()
	{
				
		for (int i = 0; i < Decks.Count; i++){
			// Validate if theres 12 cards
			if(Decks.Decks[i].CardIds.Count != 12){
				SetTextPopup($"Deck {Decks.Decks[i].Name} does not have 12 cards");
				return;
			}
			
			// Validate max 2 unique cards
			if(!HasNoMoreThanTwoDuplicates(Decks.Decks[i].CardIds)){
				SetTextPopup($"Deck {Decks.Decks[i].Name} have more than 2 duplicate cards");
				return;
			}
		}
		
		List<UpdateDeckRequest> UpdatedDecks = new List<UpdateDeckRequest>();
		
		for(int i = 0;i < Decks.Count; i++){
			UpdateDeckRequest DeckJsonObj = new UpdateDeckRequest
			{
				DeckId = Decks.Decks[i].DeckId,
				Name = Decks.Decks[i].Name,
				CardIds = Decks.Decks[i].CardIds
			};
			UpdatedDecks.Add(DeckJsonObj);
			
		}
		
		var jsonObj = new {
			decks = UpdatedDecks
		};
		
		var jsonString = JsonSerializer.Serialize(jsonObj);
		GD.Print("Sending the update deck method");
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.DECK + "/players/me/decks", Godot.HttpClient.Method.Put, jsonString, UpdateDecksResponse);
		
		// Make save button invisible
		SetSaveButtonVisibility(false);
	}
	
	// handler for updating decks
	private void UpdateDecksResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);
		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Failed to get a successful response when updating decks");
			GD.PrintErr(json);
			return;
		}


		// if active deck has been changed
		int CurrentActiveDeckId = 0;
		for(int i = 0;i < Decks.Count; i++){
			if(Decks.Decks[i].IsActive){
				CurrentActiveDeckId = Decks.Decks[i].DeckId;
			}
		}
		
		if(CurrentActiveDeckId != InitialActiveDeckId){
			var jsonObj = new {
				deck_id = CurrentActiveDeckId
			};
			
			var jsonString = JsonSerializer.Serialize(jsonObj);
			
			
			Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.DECK + "/decks/active", Godot.HttpClient.Method.Put, jsonString, UpdateActiveDeckResponse);
		}else{
			// success
			SetTextPopup("Deck has been updated successfully!");
		}

	}

	// Handler for updating active deck
	private void UpdateActiveDeckResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);
		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Failed to get a successful response when updating active deck");
			GD.PrintErr(json);
			return;
		}

		// success
		SetTextPopup("Deck has been updated successfully!");

	}
	
	// Checks that theres atmost 2 of the same card in the deck
	public bool HasNoMoreThanTwoDuplicates(List<int> cardIds)
	{
		// use dictionary to check count of each item
		var counts = new Dictionary<int, int>();

		foreach (int id in cardIds)
		{
			if (!counts.ContainsKey(id))
				counts[id] = 0;

			counts[id]++;

			if (counts[id] > 2)
				return false;
		}

		return true;
	}

	// function to make text popup visible and set the text
	private void SetTextPopup(string Text)
	{
		Control TextPopupContainer = GetNode<Control>("TextPopupContainer");
		Label TextLabel = TextPopupContainer.GetNode<Label>("TextBackgroundContainer/MarginTextContainer/TextLabel");
		TextPopupContainer.Visible = true;
		TextLabel.Text = Text;
	}

	// When pressing the "Active" OR "Set Active" Button
	private void _OnIsActiveButtonPressed()
	{
		for(int i = 0; i < Decks.Count; i++){
			Decks.Decks[i].IsActive = i==SelectedDeck;
		}
		
		// Make save button visible
		SetSaveButtonVisibility(true);
		
		// Update Active button visuals
		UpdateActiveButton();
	}
	
	// Update save button visible
	private void SetSaveButtonVisibility(bool visible)
	{
		Button SaveButton = GetNode<Button>("MainCardContainer/DeckContainer/DeckColorContainer/SaveButton");
		SaveButton.Visible = visible;
	}
	
	// Close text popup container
	private void _OnTextPopupBackgroundPressed()
	{
		Control TextPopupContainer = GetNode<Control>("TextPopupContainer");
		TextPopupContainer.Visible = false;
	}

	// when clicking level card button
	private void _OnLevelButtonPressed()
	{
		
		Control CardPopupContainerNode = GetNode<Control>("CardPopupContainer");
		Panel CardPopupBox = CardPopupContainerNode.GetNode<Panel>("CardPopupBox");
		Button LevelButton = CardPopupBox.GetNode<Button>("CardDetails/RightCardDetails/LevelContainer/LevelButton");
		
		// get card info (id, level and quantity)
		int CardId = 0;
		int Level = 1;
		int Quantity = 0;
		if (LevelButton.HasMeta("CardId"))
		{
			// do the async call here to open pack
			CardId = (int)LevelButton.GetMeta("CardId");
			Level = (int)LevelButton.GetMeta("Level");
			Quantity = (int)LevelButton.GetMeta("Quantity");
		}else{
			SetTextPopup("Error on level button pressed");

			return;
		}

		// validate if enough
		int CardCost = CardBuilder.CardCostCalculator(Level);
		int CrystalCost = CardBuilder.CrystalCostCalculator(Level);
		
		// if not enough cards/crystals
		if(CurrentPlayer.Crystal < CrystalCost || Quantity < CardCost){
			SetTextPopup("Not enough crystals or cards");
			return;
		}

		// fetch
		Network.SendRequestWithToken(NetworkManager.BASE_URL + NetworkManager.DECK + "/players/me/cards/"+CardId+"/level-up", Godot.HttpClient.Method.Post, "", LevelUpCardResponse);

	}

	// response handler for leveling cards
	private void LevelUpCardResponse(long result, long responseCode, string[] headers, byte[] body)
	{
		string json = System.Text.Encoding.UTF8.GetString(body);
		GD.Print(responseCode);
		if (result != 200 && responseCode != 200)
		{
			GD.PrintErr("Failed to get a successful response when leveling card");
			GD.PrintErr(json);
			return;
		}
		GD.Print(json);

		var data = JsonSerializer.Deserialize<LevelUpCardResponse>(json);
		int CardId = data.CardId;
		// update data
		
		// Remove from all collections if its there
		ScrollContainer CollectionScrollContainer = GetNode<ScrollContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer");
		VBoxContainer CardContainer;

		for (int x = 0; x < CollectionScrollContainer.GetChildCount() - 1; x++){
			
			HFlowContainer CollectionContainer = CollectionScrollContainer.GetChild(x) as HFlowContainer;
			for(int i = 0; i < CollectionContainer.GetChildCount(); i++){
				CardContainer = CollectionContainer.GetChild(i) as VBoxContainer;
				
				if(CardContainer.Name == "" + CardId){
					Collections[x].CardsNotInDeck[i].Quantity -= data.CardsConsumed;
					Collections[x].CardsNotInDeck[i].Level = data.NewLevel;
					// No more in collection
					if(Collections[x].CardsNotInDeck[i].Quantity <= 0){
						CardContainer.QueueFree();
						Collections[x].CardsNotInDeck.Remove(Collections[x].CardsNotInDeck[i]);
					}else{
						// update count
						Label CardCount = CardContainer.GetNode<Label>("CardCount");
						CardCount.Text = "X" + Collections[x].CardsNotInDeck[i].Quantity;

						// remove level and regenerate card
						Control CardControl = CardContainer.GetNode<Control>("CardControl");
						CardControl.GetChild(0).QueueFree();

						// Create the card itself
						var Card = CardBuilder.GenerateCardWithLevel(Collections[x].CardsNotInDeck[i].CardId, Collections[x].CardsNotInDeck[i].Level);
							
						Card.Position = new Vector2(95, 100);
						CardControl.AddChild(Card);
					}

				}
			}
		}
		// remove in deck if its pruned

		// Remove card from deck
		ScrollContainer DeckScrollContainer = GetNode<ScrollContainer>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer");

		if(data.PrunedDecks != null){
			for(int i = 0; i < data.PrunedDecks.Count; i++){
				for(int x = 0; x < Decks.Decks.Count; x++){

					if(Decks.Decks[x].DeckId == data.PrunedDecks[i].DeckId){

						// remove the card in data
						for(int y = Decks.Decks[x].CardIds.Count - 1; y >= 0; y--){
							
							if(Decks.Decks[x].CardIds[y] == CardId){
								Decks.Decks[x].CardIds.Remove(Decks.Decks[x].CardIds[y]);
								// remove from Cards
								Decks.Decks[x].Cards.Remove(Decks.Decks[x].Cards[y]);

								// remove the card visually
								HFlowContainer DeckContainer = DeckScrollContainer.GetChild(x) as HFlowContainer;
								CardContainer = DeckContainer.GetChild(y) as VBoxContainer;
								CardContainer.QueueFree();

								// make sure remove correct number of copies
								data.PrunedDecks[i].Removed -= 1;
								
								if(data.PrunedDecks[i].Removed == 0){
									break;
								}
							}
						}

						// no point loop through the deck list since removed for the deck id alr
						break;
					}

				}
			}
		}
		// update deck card level
		for(int x = 0; x < Decks.Decks.Count; x++){
			for(int y = 0; y < Decks.Decks[x].Cards.Count; y++){
				if(Decks.Decks[x].Cards[y].CardId == CardId){
					Decks.Decks[x].Cards[y].Level = data.NewLevel;

					// update card visually
					HFlowContainer DeckContainer = DeckScrollContainer.GetChild(x) as HFlowContainer;
					CardContainer = DeckContainer.GetChild(y) as VBoxContainer;

					// remove level and regenerate card
					Control CardControl = CardContainer.GetNode<Control>("CardControl");
					CardControl.GetChild(0).QueueFree();

					// Create the card itself
					var Card = CardBuilder.GenerateCardWithLevel(CardId, Decks.Decks[x].Cards[y].Level);
							
					Card.Position = new Vector2(95, 100);
					CardControl.AddChild(Card);
				}
			}
		}

		// text popup for pruned deck
		if(data.PrunedDecks != null && data.PrunedDecks.Count > 0){
			SetTextPopup("Deck has been changed, please ensure all decks have 12 cards.");
		}
		
		// update current popup
		OpenCardPopup(CardId, data.NewLevel, data.QuantityLeft);

		// update crystal
		UpdateCrystals(data.CrystalsLeft);

	}
}
