namespace FSDClient.home;

using Godot;
using System;
using FSDClient.autoLoad;
using System.Collections.Generic;

public partial class Home : Control
{
	private static PackedScene CardScene = GD.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");
	private volatile bool _searching = false;
	private PlayerStateManager CurrentPlayer { get; set; }
	private Dictionary<string, Texture2D> PackTextures = new Dictionary<string, Texture2D>
	{
		{"None", GD.Load<Texture2D>("res://assets/cards/pack.png")},
		{"Common", GD.Load<Texture2D>("res://assets/cards/CommonPack.png")},
		{"Rare", GD.Load<Texture2D>("res://assets/cards/RarePack.png")},
		{"Epic", GD.Load<Texture2D>("res://assets/cards/EpicPack.png")},
		{"Legendary", GD.Load<Texture2D>("res://assets/cards/LegendaryPack.png")}
	};
	
	private Dictionary<string, string> PackData = new Dictionary<string, string>
	{
		{"Pack1", "None"},
		{"Pack2", "None"},
		{"Pack3", "None"},
		{"Pack4", "None"}
	};
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		try
		{
			CurrentPlayer = PlayerStateManager.Instance;
			GD.Print(CurrentPlayer.ToString());
			if (CurrentPlayer.PlayerData == null)
			{
				GD.Print("The PlayerData is empty");
				var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
				//GameStateManager.ChangeGameState(GameState.LOGIN);
			}
			
			// Function to get pack, update param to accept user data
			GetPacks();

		}
		catch (Exception e)
		{
			GD.PrintErr("Whoops ", e);
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//	Press Battle button
	public async void _OnBattleButtonPressed()
	{

		ColorRect loadingNode = GetNode<ColorRect>("Loading");

		// start animation
		AnimatedSprite2D LoadingSprite = GetNode<AnimatedSprite2D>("Loading/LoadingAnimation");
		LoadingSprite.Play("loading");  // Replace "default" with your animation name

		// Turn it ON (make visible)
		loadingNode.Visible = true;
		_searching = true;

		// find game stuff
		while (_searching)
		{
			// 1 SECOND TIMEOUT - Godot way
			await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

			// if it did not get canceled
			if (_searching)
			{
				// found match
				_searching = false;
				var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
				GameStateManager.ChangeGameState(GameState.INGAMEMODE);
			}
		}
	}

	// Press card button
	public void _OnCardButtonPressed()
	{
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.CARDSCREEN);
	}

	// Press cancel button when finding match
	public void _OnCancelButtonPressed()
	{
		_searching = false;
		Control loadingNode = GetNode<ColorRect>("Loading");

		// stop animation
		AnimatedSprite2D LoadingSprite = GetNode<AnimatedSprite2D>("Loading/LoadingAnimation");
		LoadingSprite.Stop();  // Replace "default" with your animation name

		// Turn it OFF (make visible false)
		loadingNode.Visible = false;
	}

	// Press friend button (to show friend list)
	public void _OnFriendButtonPressed()
	{
		Control FriendPopupContainerNode = GetNode<Control>("FriendPopupContainer");

		// Turn it ON (make visible)
		FriendPopupContainerNode.Visible = true;
	}

	// Button press function when clicking background to close friends popup
	public void _OnFriendPopupBackgroundPressed()
	{
		CloseFriendPopup();
	}

	// Button press function when clicking X to close friends popup
	public void _OnCloseFriendButtonPressed()
	{
		CloseFriendPopup();
	}

	// Function to close friend popup
	public void CloseFriendPopup()
	{
		Control FriendPopupContainerNode = GetNode<Control>("FriendPopupContainer");

		// Turn it ON (make visible)
		FriendPopupContainerNode.Visible = false;
	}

	// Function to get packs and set it visually
	private void GetPacks()
	{
		// Make each pack clickable
		Button PackSlot1 = GetNode<Button>("Packs/PackSlot1");
		Button PackSlot2 = GetNode<Button>("Packs/PackSlot2");
		Button PackSlot3 = GetNode<Button>("Packs/PackSlot3");
		Button PackSlot4 = GetNode<Button>("Packs/PackSlot4");

		// All call the same function, but with different index
		PackSlot1.Pressed += () => OnPackSlotPressed(1);
		PackSlot2.Pressed += () => OnPackSlotPressed(2);
		PackSlot3.Pressed += () => OnPackSlotPressed(3);
		PackSlot4.Pressed += () => OnPackSlotPressed(4);


		var Packs = GetNode<HFlowContainer>("Packs");
		
		// update to actual ASYNC function call
		PackData["Pack1"] = "Legendary";
		PackData["Pack2"] = "Common";
		PackData["Pack3"] = "Epic";
		
		for (int i = 0; i< Packs.GetChildCount(); i++)
		{
			var PackSlot = Packs.GetChild(i) as Button;
			
			var PackImage = PackSlot.GetNode<TextureRect>("PackImage");
			// see what kind of pack and assign the correct texture
			if (PackTextures.TryGetValue(PackData["Pack" + (i + 1)], out var texture))
			{
				PackImage.Texture = texture;
			}
		}
	}
	
	// When the pack slot is being pressed
	private void OnPackSlotPressed(int index)
	{
		
		Button PackSlot = GetNode<Button>("Packs/PackSlot" + index);
		
		TextureRect Pack = PackSlot.GetNode<TextureRect>("PackImage");
		
		// if got pack then call
		if(PackData["Pack"+ index] != "None")
		{
			// do the async call here to open pack
			string[] cards = {"A","B","C"};
			
			// open pack visuals
			ColorRect PacksPopupContainer = GetNode<ColorRect>("PacksPopupContainer");
			Control IndivCardContainer = PacksPopupContainer.GetNode<Control>("IndivCardContainer");
			HFlowContainer FinalCardResult = PacksPopupContainer.GetNode<HFlowContainer>("FinalCardResult");

			// create a screen showing all cards (Visibility off)
			FinalCardResult.Visible = false;
			
			// set generate cards one on top of another
			for (int i = 0;i < cards.Length; i++){
				VBoxContainer CardContainer = CreateCard("Legendary", cards[i], i);
				
				// Set visibility to false (for all but 1st)
				if(i != 0){
					CardContainer.Visible = false;
				}
				
				IndivCardContainer.AddChild(CardContainer);
				
				VBoxContainer FinalCardContainer = CreateCard("Legendary", cards[i], i);
				FinalCardResult.AddChild(FinalCardContainer);
			}
			
			// Turn it ON (make visible)
			PacksPopupContainer.Visible = true;
			
			// Delete the pack (TODO: change it to async call)
			PackData["Pack"+ index] = "None";
			Pack.Texture = PackTextures["None"];
		}
	}
	
	// Function to create a card for display
	private static VBoxContainer CreateCard(string rarity, string card, int count){
		var CardContainer = new VBoxContainer();
		CardContainer.CustomMinimumSize = new Vector2(226, 346);
		
		// add Rarity label into CardContainer
		var RarityLabel = new Label();
		RarityLabel.Text = rarity;
		RarityLabel.AddThemeFontSizeOverride("font_size", 39);
		RarityLabel.AddThemeColorOverride("font_color", new Color(255, 255, 255));
		RarityLabel.HorizontalAlignment = HorizontalAlignment.Center;
		CardContainer.AddChild(RarityLabel);
		
		// Create CardControl with card inside
		var CardControl = new Control();
		CardControl.CustomMinimumSize = new Vector2(200, 230);
		CardControl.Position = new Vector2(13, 58);
		CardControl.SizeFlagsHorizontal = (Control.SizeFlags)SizeFlags.ShrinkCenter;
		
		// Create the card itself
		var Card = CardScene.Instantiate<Node2D>();
		Card.Position = new Vector2(100, 115);
		
		CardControl.AddChild(Card);
		CardContainer.AddChild(CardControl);
		
		// Add CardCountLabel and add into CardContainer
		var CardCountLabel = new Label();
		CardCountLabel.Text = "X" + count;
		CardCountLabel.AddThemeFontSizeOverride("font_size", 39);
		CardCountLabel.AddThemeColorOverride("font_color", new Color(255, 255, 255));
		CardCountLabel.HorizontalAlignment = HorizontalAlignment.Center;
		CardContainer.AddChild(CardCountLabel);
		
		return CardContainer;
	}
	// When clicking the background
	public void _OnPacksPopupBackgroundPressed()
	{
		
		ColorRect PacksPopupContainer = GetNode<ColorRect>("PacksPopupContainer");
		Control IndivCardContainer = PacksPopupContainer.GetNode<Control>("IndivCardContainer");
		HFlowContainer FinalCardResult = PacksPopupContainer.GetNode<HFlowContainer>("FinalCardResult");

		// if still have card to show
		if(IndivCardContainer.GetChildCount() > 1){
			// make bottom card visible	
			VBoxContainer NewCardContainer = IndivCardContainer.GetChild(1) as VBoxContainer;
			NewCardContainer.Visible = true;
			
			// move card animation
			VBoxContainer CardContainer = IndivCardContainer.GetChild(0) as VBoxContainer;
			SlideOutAndDelete(CardContainer);
		}else if (IndivCardContainer.GetChildCount() == 1){
		// no more card to show afterwards
			// move card animation
			VBoxContainer CardContainer = IndivCardContainer.GetChild(0) as VBoxContainer;
			SlideOutAndDelete(CardContainer);
			
			// display all cards (set to visible)
			FinalCardResult.Visible = true;
		}else{
		// Means showing all cards already
			// Do clean up and remove all info
			FinalCardResult.Visible = false;
			// Need delete all items to prepare if user open another pack
			for(int i = FinalCardResult.GetChildCount() - 1; i >= 0; i--)
			{
				FinalCardResult.GetChild(i).QueueFree();
			}
		
			PacksPopupContainer.Visible = false;
		}
			
	}
	
	// Animation to slide out and delete
	private static void SlideOutAndDelete(Node node)
	{
		var tween = node.CreateTween();
		tween.TweenProperty(node, "position:x", 200f, 0.3f)
			 .SetEase(Tween.EaseType.Out);
		tween.TweenCallback(Callable.From(() => node.QueueFree()));
	}
}
