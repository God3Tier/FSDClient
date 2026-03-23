using Godot;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
	
public partial class CardManagement : Control
{
	public PackedScene CardScene = GD.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");
	private List<FlowContainer> Decks = new List<FlowContainer>();
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		generate_collection();
		generate_decks();
	}
	
	// Function to generate card collection
	public void generate_collection()
	{
		// insert cards into collection
		var CollectionContainer = GetNode<FlowContainer>("MainCardContainer/CollectionContainer/CollectionColorContainer/ScrollContainer/CollectionContainer");
		
		// Generate 13 items
		for (int i = 0; i < 13; i++)
		{
			var CardContainer = new VBoxContainer();
				
			// Create control that gives the card its size
			var CardControl = new Control();
			CardControl.CustomMinimumSize = new Vector2(200, 230);
			
			// Create the card itself
			var Card = CardScene.Instantiate<Node2D>();
			Card.Position = new Vector2(100, 115);
			CardControl.AddChild(Card);
			
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
	
	// Function to generate Decks
	public async Task generate_decks()
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

	public void _on_back_button_pressed()
	{
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.HOMESCREEN);
	}
	
	public void _on_card_popup_background_pressed()
	{
		close_card_popup();
	}	
	
	public void _on_close_card_button_pressed()
	{
		close_card_popup();
	}	
	
	public void close_card_popup()
	{
		Control CardPopupContainerNode = GetNode<Control>("CardPopupContainer");
		
		// Turn it OFF (make visible false)
		CardPopupContainerNode.Visible = false;
	}	
}
