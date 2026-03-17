namespace FSDClient.home;

using Godot;
using System;
using FSDClient.autoLoad;

public partial class Home : Control
{
	private volatile bool _searching = false;
	private PlayerStateManager CurrentPlayer { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CurrentPlayer = PlayerStateManager.Instance;
		
		if (CurrentPlayer.Token == null)
		{
			
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public async void _on_battle_button_pressed()
	{
		
		Control loadingNode = GetNode<ColorRect>("Loading");
		
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
			if(_searching){
				// found match
				_searching = false;
				var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
				GameStateManager.ChangeGameState(GameState.INGAMEMODE);
			}
		}
	}
	
	public void _on_card_button_pressed()
	{
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.CARDSCREEN);
	}

	public void _on_cancel_button_pressed()
	{
		_searching = false;
		Control loadingNode = GetNode<ColorRect>("Loading");
		
		// stop animation
		AnimatedSprite2D LoadingSprite = GetNode<AnimatedSprite2D>("Loading/LoadingAnimation");
		LoadingSprite.Stop();  // Replace "default" with your animation name
		
		// Turn it OFF (make visible false)
		loadingNode.Visible = false;
	}	
	
}
