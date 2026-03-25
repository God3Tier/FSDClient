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
			CardButtonOverlay.Pressed += () => OnCollectionCardPressed(index);
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
	
	private void OnCollectionCardPressed(int index){
		var CollectionContainer = GetNode<FlowContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer/CollectionContainer");
		
		VBoxContainer CardContainer = CollectionContainer.GetChild(index) as VBoxContainer;
		
		Control CardControl = CardContainer.GetNode<Control>("CardControl");
		Vector2 pos = CardContainer.Position;
		
		// Check if card button exists
		int ButtonIndex = CheckCardButton(pos.X, pos.Y);
		
		// Button does not exist yet
		if(ButtonIndex == -1){
			// Create button
			CreateCardButton(pos.X, pos.Y, index);
			GD.Print(pos.Y);
			RefreshScrollbar();
		}else{
			// Else deletes it
			DeleteCardButton(ButtonIndex);
		}
	}
	
	private void RefreshScrollbar(){
		GD.Print("TotalHeightCA");
		ScrollContainer Scroll = GetNode<ScrollContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer");
		HFlowContainer Content = Scroll.GetNode<HFlowContainer>("CollectionContainer");

		// Calculate total Content height
		GD.Print("TotalHeightA");
		float TotalHeight = 0;
		foreach(Control Child in Content.GetChildren().OfType<Control>())
		{
			Vector2 pos = Child.Position;
			if(TotalHeight < pos.Y){
				TotalHeight = pos.Y;
			}
		}
		GD.Print(TotalHeight);

		float DesiredHeight = TotalHeight + 300;
		GD.Print(DesiredHeight);
		Content.CustomMinimumSize = new Vector2(Scroll.Size.X, DesiredHeight);
		Scroll.QueueSort();  // Refresh layout
	}
	
	// Function to check if card button set exists, return the index of the element child
	private int CheckCardButton(float X, float Y){
		var ButtonContainer = GetNode<Control>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer/ButtonContainer");
		
		for (int i = 0; i < ButtonContainer.GetChildCount(); i++){
			Control ButtonSet = ButtonContainer.GetChild(i) as Control;
			// use position to find the first button
			if(ButtonSet.Position == new Vector2(X + 10, Y + 200)){
				return i;
			}
		}
		return -1;
	}
	
	private void DeleteCardButton(int index){
		var ButtonContainer = GetNode<Control>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer/ButtonContainer");
		
		Control ButtonSet = ButtonContainer.GetChild(index) as Control;
		ButtonSet.QueueFree();
	}
	
	// Function to create the 2 buttons
	private void CreateCardButton(float X, float Y, int index){
		var ButtonSet = new Control();
		ButtonSet.Position = new Vector2(X + 10, Y + 200);
		
		// Add the 2 button (more info & add to deck)
		var MoreInfoButton = new Button();
		var AddToDeckButton = new Button();
		
		MoreInfoButton.CustomMinimumSize = new Vector2(170, 40);
		MoreInfoButton.Position = new Vector2(0, 0);
		MoreInfoButton.Text = "More Info";
		MoreInfoButton.ZIndex = 3;
		MoreInfoButton.AddThemeFontSizeOverride("font_size", 20);
		MoreInfoButton.AddThemeColorOverride("font_color", new Color(255, 255, 255));
		MoreInfoButton.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
			
		AddToDeckButton.CustomMinimumSize = new Vector2(170, 40);
		AddToDeckButton.Position = new Vector2(0, 40);
		AddToDeckButton.Text = "Add";
		AddToDeckButton.ZIndex = 3;
		AddToDeckButton.AddThemeFontSizeOverride("font_size", 20);
		AddToDeckButton.AddThemeColorOverride("font_color", new Color(255, 255, 255));
		AddToDeckButton.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
		
		StyleBox button = GD.Load<StyleBox>("res://styles/button.tres");
		StyleBox ButtonHover = GD.Load<StyleBox>("res://styles/button.tres");
		StyleBox ButtonPressed = GD.Load<StyleBox>("res://styles/button.tres");

		MoreInfoButton.AddThemeStyleboxOverride("normal", button);
		MoreInfoButton.AddThemeStyleboxOverride("hover", ButtonHover);
		MoreInfoButton.AddThemeStyleboxOverride("pressed", ButtonPressed);
			
		AddToDeckButton.AddThemeStyleboxOverride("normal", button);
		AddToDeckButton.AddThemeStyleboxOverride("hover", ButtonHover);
		AddToDeckButton.AddThemeStyleboxOverride("pressed", ButtonPressed);
			
		// Connect pressed to a function
		MoreInfoButton.Pressed += () => OpenCardPopup(index);
		AddToDeckButton.Pressed += () => OpenCardPopup(index);
		
		ButtonSet.AddChild(MoreInfoButton);
		ButtonSet.AddChild(AddToDeckButton);
	
		var ButtonContainer = GetNode<Control>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer/ButtonContainer");
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
			
			// Generate 12 items
			for (int i = 0; i < 10 + j; i++)
			{
				// Create control that gives the card its size
				var CardControl = new Control();
				CardControl.CustomMinimumSize = new Vector2(200, 230);
				
				// Create the card itself
				var Card = CardScene.Instantiate<Node2D>();
				Card.Position = new Vector2(100, 115);
				CardControl.AddChild(Card);
				
				// Add the card into the main container
				DeckContainer.AddChild(CardControl);
			}
			
			
			// load it all in
			var ScrollContainer = GetNode<ScrollContainer>("MainCardContainer/DeckContainer/DeckColorContainer/ScrollContainer");
			ScrollContainer.AddChild(DeckContainer);
			
			// First deck will be visible, rest won't
			if (j != 0){
				DeckContainer.Visible = false;
			}
			
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
