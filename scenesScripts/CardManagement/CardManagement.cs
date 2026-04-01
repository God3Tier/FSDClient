using Godot;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using System.Linq;
	
public class Collection
	{
		public List<CardQuantity> Cards { get; set; } = new();
	}

public class CardQuantity
	{
		public int CardID { get; set; } = new();
		public int Quantity { get; set; } = new();
	}

public class Deck
	{
		public int DeckID { get; set; }
		public string Name { get; set; } = "";
		public List<int> CardIDs { get; set; } = new();
		public List<CardInfo> Cards { get; set; } = new();
		public bool IsActive { get; set; } = false;
	}

public class CardInfo
	{
		public int CardID { get; set; }
		public int Position { get; set; }
	}
	
public partial class CardManagement : Control
{
	private PackedScene CardScene = GD.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");
	private List<Deck> Decks;
	private List<Collection> Collections;
	private int SelectedDeck = 0;
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Collections = FetchCollections();
		GenerateCollection();
		Decks = FetchDecks();
		GenerateDecks();
		UpdateActiveButton();
	}
	
	// TODO: Function to fetch collections from backend
	private List<Collection> FetchCollections(){
		var Collections = new List<Collection>
		{
			new Collection
			{
				Cards = new List<CardQuantity>
				{
					new CardQuantity { CardID = 2, Quantity = 3 },
					new CardQuantity { CardID = 3, Quantity = 3 },
					new CardQuantity { CardID = 4, Quantity = 2 },
					new CardQuantity { CardID = 5, Quantity = 2 },
					new CardQuantity { CardID = 6, Quantity = 2 },
					new CardQuantity { CardID = 7, Quantity = 2 },
					new CardQuantity { CardID = 8, Quantity = 2 },
					new CardQuantity { CardID = 9, Quantity = 2 },
					new CardQuantity { CardID = 10, Quantity = 2 }
				}
			},

			new Collection
			{
				Cards = new List<CardQuantity>
				{
					new CardQuantity { CardID = 2, Quantity = 1 },
					new CardQuantity { CardID = 3, Quantity = 2 },
					new CardQuantity { CardID = 4, Quantity = 2 },
					new CardQuantity { CardID = 5, Quantity = 2 },
					new CardQuantity { CardID = 6, Quantity = 2 },
					new CardQuantity { CardID = 7, Quantity = 2 },
					new CardQuantity { CardID = 8, Quantity = 2 },
					new CardQuantity { CardID = 9, Quantity = 2 },
					new CardQuantity { CardID = 10, Quantity = 2 }
				}
			}
		};
		
		return Collections;
	}
	
	// Function to generate card collection
	public void GenerateCollection()
	{
		// Generate decks
		for (int j = 0; j < Collections.Count; j++)
		{
			int CollectionIndex = j;
			var CollectionContainer = new HFlowContainer();
			CollectionContainer.CustomMinimumSize = new Vector2(1000, 650);
			CollectionContainer.Name = "CollectionContainer";
			
			// Generate collection cards
			for (int i = 0; i < Collections[j].Cards.Count; i++)
			{
				// to pass the values over to any functions (passing i will always pass max)
				int CardIndex = i;
				
				var CardContainer =  CreateCardWithLabel("Collection", Collections[CollectionIndex].Cards[CardIndex].CardID, Collections[j].Cards[i].Quantity);
				
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
	private void OnCardPressed(string Location, int CardID, Vector2 pos){
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
			CreateCardButton(pos.X, pos.Y, CardID, Location, ButtonContainer);
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
	private void CreateCardButton(float X, float Y, int CardID, string Location, Control ButtonContainer){
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
		InfoButton.Pressed += () => OpenCardPopup(CardID);
		
		if(Location == "Collection"){
			AddOrRemoveDeckButton.Pressed += () => AddIntoDeck(CardID);
		}else{
			AddOrRemoveDeckButton.Pressed += () => RemoveFromDeck(CardID, X, Y);
		}
		
		ButtonSet.AddChild(InfoButton);
		ButtonSet.AddChild(AddOrRemoveDeckButton);
	
		ButtonContainer.AddChild(ButtonSet);
	}
	
	// TODO: Function to fetch deck from backend
	private List<Deck> FetchDecks(){
		var Decks = new List<Deck>
			{
				new Deck
				{
					DeckID = 1,
					Name = "name1",
					CardIDs = new List<int> { 1, 2 },
					Cards = new List<CardInfo>
					{
						new CardInfo { CardID = 1, Position = 1 },
						new CardInfo { CardID = 2, Position = 2 }
					},
					IsActive = false,
				},
				new Deck
				{
					DeckID = 2,
					Name = "name2",
					CardIDs = new List<int> { 1, 2, 3 },
					Cards = new List<CardInfo>
					{
						new CardInfo { CardID = 1, Position = 1 },
						new CardInfo { CardID = 2, Position = 2 },
						new CardInfo { CardID = 3, Position = 3 }
					},
					IsActive = true,
				}
			};
		
		return Decks;
	}
	
	// Function to generate Decks
	private async Task GenerateDecks()
	{
		// Generate decks
		for (int j = 0; j < Decks.Count; j++)
		{
			int DeckIndex = j;
			// insert cards into deck
			var DeckContainer = new HFlowContainer();
			DeckContainer.CustomMinimumSize = new Vector2(610, 660);
			DeckContainer.Name = "DeckContainer";
			
			// Generate Cards
			for (int i = 0; i < Decks[j].Cards.Count; i++)
			{
				// to pass the values over to any functions (passing i will always pass max)
				int CardIndex = i;
				
				// Create control that gives the card its size
				var CardContainer = CreateCard("Deck", Decks[DeckIndex].Cards[CardIndex].CardID);
				
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
		
		// Chain to tabs AFTER decks finish (main thread safe)
		CallDeferred(MethodName.CreateDynamicTabs);
	}
	
	// Create Card with count (For collection)
	private VBoxContainer CreateCardWithLabel(string Location, int CardID, int Quantity)
	{
		var CardContainer = CreateCard(Location, CardID);
		
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
	private VBoxContainer CreateCard(string Location, int CardID)
	{
		// Create control that gives the card its size
		var CardContainer = new VBoxContainer();
		CardContainer.CustomMinimumSize = new Vector2(190, 200);
		CardContainer.Name = "" + CardID;
		
		// Create control that gives the card its size
		var CardControl = new Control();
		CardControl.CustomMinimumSize = new Vector2(190, 200);
		CardControl.Name = "CardControl";
				
		// Create the card itself
		var Card = CardScene.Instantiate<Node2D>();
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

		CardButtonOverlay.Pressed += () => OnCardPressed(Location, CardID, CardContainer.Position);
		CardControl.AddChild(CardButtonOverlay);
			
		// Add the card into the container
		CardContainer.AddChild(CardControl);
		
		
		return CardContainer;
	}
	
	// function to add a card into deck (called by button)
	private void AddIntoDeck(int CardID)
	{
		// Remove collection		
		ScrollContainer CollectionScrollContainer = GetNode<ScrollContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer");
		HFlowContainer CollectionContainer = CollectionScrollContainer.GetChild(SelectedDeck) as HFlowContainer;
		VBoxContainer CardContainer;
		for(int i = 0; i < CollectionContainer.GetChildCount(); i++){
			CardContainer = CollectionContainer.GetChild(i) as VBoxContainer;
			
			if(CardContainer.Name == "" + CardID){
				Collections[SelectedDeck].Cards[i].Quantity -= 1;
				// No more in collection
				if(Collections[SelectedDeck].Cards[i].Quantity <= 0){
					CardContainer.QueueFree();
					Collections[SelectedDeck].Cards.Remove(Collections[SelectedDeck].Cards[i]);
				}else{
				// just update count
					Label CardCount = CardContainer.GetNode<Label>("CardCount");
					CardCount.Text = "X" + Collections[SelectedDeck].Cards[i].Quantity;
				}
			}
		}
		
		// Add into deck data
		Decks[SelectedDeck].CardIDs.Add(CardID);
		Decks[SelectedDeck].Cards.Add(new CardInfo {CardID = CardID, Position = Decks[SelectedDeck].Cards.Count});
		
		// Add visually
		ScrollContainer DeckScrollContainer = GetNode<ScrollContainer>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer");
		HFlowContainer DeckContainer = DeckScrollContainer.GetChild(SelectedDeck) as HFlowContainer;
		CardContainer = CreateCard("Deck", CardID);
		DeckContainer.AddChild(CardContainer);
		
		// Make save button visible
		SetSaveButtonVisibility(true);
		
		// Reset buttons if not it will be out of position
		RemoveButtonContainers();
	}
	
	private void RemoveFromDeck(int CardID, float X, float Y){
		// Remove card from deck
		ScrollContainer DeckScrollContainer = GetNode<ScrollContainer>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer");
		HFlowContainer DeckContainer = DeckScrollContainer.GetChild(SelectedDeck) as HFlowContainer;
		VBoxContainer CardContainer;
		for(int i = 0; i < DeckContainer.GetChildCount(); i++){
			CardContainer = DeckContainer.GetChild(i) as VBoxContainer;
			Vector2 pos = CardContainer.Position;
			if(pos.X == X && pos.Y == Y){
				// remove from cardID
				Decks[SelectedDeck].CardIDs.Remove(Decks[SelectedDeck].CardIDs[i]);
				// remove from Cards
				Decks[SelectedDeck].Cards.Remove(Decks[SelectedDeck].Cards[i]);
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
			if(CardContainer.Name == "" + CardID){
				Collections[SelectedDeck].Cards[i].Quantity += 1;
				
				// Update count
				Label CardCount = CardContainer.GetNode<Label>("CardCount");
				CardCount.Text = "X" + Collections[SelectedDeck].Cards[i].Quantity;
				
				CardExist = true;
				
			}
		}
		
		// if card did not exist in collection
		if(!CardExist){
			// insert into collection data
			Collections[SelectedDeck].Cards.Add(new CardQuantity {CardID = CardID, Quantity = 1});
			
			// Insert visually
			CardContainer = CreateCardWithLabel("Collection", CardID, 1);
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
		if(Decks[SelectedDeck].IsActive){
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
	
	private void OpenCardPopup(int CardID)
	{
		Control CardPopupContainerNode = GetNode<Control>("CardPopupContainer");
		
		// Turn it OFF (make visible false)
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
			if(Decks[i].CardIDs.Count != 12){
				SetErrorPopup($"Deck {Decks[i].Name} does not have 12 cards");
				return;
			}
			
			// Validate max 2 unique cards
			if(!HasNoMoreThanTwoDuplicates(Decks[i].CardIDs)){
				SetErrorPopup($"Deck {Decks[i].Name} have more than 2 duplicate cards");
				return;
			}
		}
		// TODO: fetch to save
		
		// Make save button invisible
		SetSaveButtonVisibility(false);
	}
	
	// Checks that theres atmost 2 of the same card in the deck
	public bool HasNoMoreThanTwoDuplicates(List<int> cardIDs)
	{
		// use dictionary to check count of each item
		var counts = new Dictionary<int, int>();

		foreach (int id in cardIDs)
		{
			if (!counts.ContainsKey(id))
				counts[id] = 0;

			counts[id]++;

			if (counts[id] > 2)
				return false;
		}

		return true;
	}

	// function to make error popup visible and set the text
	private void SetErrorPopup(string Text)
	{
		Control ErrorPopupContainer = GetNode<Control>("ErrorPopupContainer");
		Label ErrorLabel = ErrorPopupContainer.GetNode<Label>("ErrorBackgroundContainer/MarginErrorContainer/ErrorLabel");
		ErrorPopupContainer.Visible = true;
		ErrorLabel.Text = Text;
	}

	// When pressing the "Active" OR "Set Active" Button
	private void _OnIsActiveButtonPressed()
	{
		for(int i = 0; i < Decks.Count; i++){
			Decks[i].IsActive = i==SelectedDeck;
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
	
	private void _OnErrorPopupBackgroundPressed(){
		Control ErrorPopupContainer = GetNode<Control>("ErrorPopupContainer");
		ErrorPopupContainer.Visible = false;
	}
}
