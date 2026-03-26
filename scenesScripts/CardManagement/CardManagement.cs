using Godot;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using System.Linq;
	
public partial class CardManagement : Control
{
	private PackedScene CardScene = GD.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");
	private List<FlowContainer> Decks = new List<FlowContainer>();
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GenerateCollection();
		GenerateDecks();
	}
	
	// Function to generate card collection
	public void GenerateCollection()
	{
		// insert cards into collection
		var CollectionContainer = GetNode<FlowContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer/CollectionContainer");
		
		// Generate 13 items
		for (int i = 0; i < 13; i++)
		{
			// to pass the values over to any functions (passing i will always pass max)
			int index = i;
			
			var CardContainer = new VBoxContainer();
				
			// Create control that gives the card its size
			var CardControl = new Control();
			CardControl.Name = "CardControl";
			CardControl.CustomMinimumSize = new Vector2(190, 200);
			
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

			// Connect pressed to a function
			CardButtonOverlay.Pressed += () => OnCardPressed(index, "Collection");
			CardControl.AddChild(CardButtonOverlay);
			
			// Add the card into the container
			CardContainer.AddChild(CardControl);
			
			var CardCount = new Label();
			CardCount.Text = "X1";
			CardCount.AddThemeFontSizeOverride("font_size", 23);
			CardCount.AddThemeColorOverride("font_color", new Color(0, 0, 0));
			CardCount.HorizontalAlignment = HorizontalAlignment.Center;
			
			// Add the card count label into the container
			CardContainer.AddChild(CardCount);
			
			// Add the card into the main container
			CollectionContainer.AddChild(CardContainer);
			
		}
	}
	
	// When a Card is being pressed
	private void OnCardPressed(int index, string location){
		Vector2 pos;
		Control ButtonContainer;
		Panel AllCardsContainer;
		if(location == "Collection"){
			AllCardsContainer = GetNode<Panel>("MainCardContainer/CollectionContainer/CollectionColorContainer");
			FlowContainer CardFlowContainer = AllCardsContainer.GetNode<FlowContainer>("ScrollContainer/CollectionContainer");
			VBoxContainer CardContainer = CardFlowContainer.GetChild(index) as VBoxContainer;
			pos = CardContainer.Position;
			
			ButtonContainer = AllCardsContainer.GetNode<Control>("ScrollContainer/ButtonContainer");
		}else if(location == "Deck"){
			AllCardsContainer = GetNode<Panel>("MainCardContainer/DeckContainer/DeckColorContainer");
			
			// Find the cards being displayed (as decks have multiple but only 1 displayed) (i-1 to account for button overlays)
			ScrollContainer Scroll = AllCardsContainer.GetNode<ScrollContainer>("ScrollContainer");
			FlowContainer CardFlowContainer = null;
			for (int i = 0; i < Scroll.GetChildCount() - 1; i++){
				CardFlowContainer = Scroll.GetChild(i) as HFlowContainer;
			
				if(CardFlowContainer.Visible == true){
					break;
				}
			}
			
			Control CardControl = CardFlowContainer.GetChild(index) as Control;
			pos = CardControl.Position;
			
			ButtonContainer = Scroll.GetNode<Control>("ButtonContainer");
		}else{
			return;
		}
		
			
		// Check if card button exists
		int ButtonIndex = CheckCardButton(pos.X, pos.Y, ButtonContainer);
		// Button does not exist yet
		if(ButtonIndex == -1){
			// Create button
			CreateCardButton(pos.X, pos.Y, index, location, ButtonContainer);
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
		
		// Find the cards being displayed (as decks have multiple but only 1 displayed) (i-1 to account for button overlays)
		for (int i = 0; i < Scroll.GetChildCount() - 1; i++){
			Content = Scroll.GetChild(i) as HFlowContainer;
			
			if(Content.Visible == true){
				break;
			}
		}

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
	private void CreateCardButton(float X, float Y, int index, string location, Control ButtonContainer){
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
		if(location == "Collection"){
			AddOrRemoveDeckButton.Text = "Add";
		}else if(location == "Deck"){
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
		InfoButton.Pressed += () => OpenCardPopup(index);
		
		if(location == "Collection"){
			AddOrRemoveDeckButton.Pressed += () => OpenCardPopup(index);
		}else if(location == "Deck"){
			AddOrRemoveDeckButton.Pressed += () => OpenCardPopup(index);
		}
		AddOrRemoveDeckButton.Pressed += () => OpenCardPopup(index);
		
		ButtonSet.AddChild(InfoButton);
		ButtonSet.AddChild(AddOrRemoveDeckButton);
	
		ButtonContainer.AddChild(ButtonSet);
	}
	
	// Function to generate Decks
	private async Task GenerateDecks()
	{
		// Generate 3 decks
		for (int j = 0; j < 3; j++)
		{
			// insert cards into deck
			var DeckContainer = new HFlowContainer();
			DeckContainer.CustomMinimumSize = new Vector2(610, 660);
			DeckContainer.Name = "DeckContainer";
			
			// Generate 12 items
			for (int i = 0; i < 10 + j; i++)
			{
				// to pass the values over to any functions (passing i will always pass max)
				int index = i;
				// Create control that gives the card its size
				var CardControl = new Control();
				CardControl.CustomMinimumSize = new Vector2(190, 200);
				
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

				// Connect pressed to a function
				CardButtonOverlay.Pressed += () => OnCardPressed(index, "Deck");
				CardControl.AddChild(CardButtonOverlay);
			
				// Add the card into the main container
				DeckContainer.AddChild(CardControl);
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
			
			Decks.Add(DeckContainer);
		}
		
		// Chain to tabs AFTER decks finish (main thread safe)
		CallDeferred(MethodName.CreateDynamicTabs);
	}
	
	// create dynamic amount of tabs based on deck count
	private void CreateDynamicTabs()
	{
	 	var DeckTab = GetNode<TabBar>("MainCardContainer/DeckContainer/DeckTab");
		
		// Create tabs
		for(int i = 0; i < Decks.Count; i++){
			DeckTab.AddTab("Deck " + (i + 1));
		}
		
		DeckTab.TabChanged += OnTabChanged;
	}
	
	// handle changing of tabs for decks
	private void OnTabChanged(long tabIndex)
	{
		// Show/hide content based on tab
		for(int i = 0; i < Decks.Count; i++)
		{
			Decks[i].Visible = (i == tabIndex);
		}
		
		// Delete all past Card Buttons ("Info, "Remove") as there should not be any in new decks
		Control ButtonContainer = GetNode<Control>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer/ButtonContainer");
		
		for(int i = 0; i < ButtonContainer.GetChildCount(); i++){
			ButtonContainer.GetChild(i).QueueFree();
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
	
	private void OpenCardPopup(int index)
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
}
