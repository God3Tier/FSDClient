namespace FSDClient.home;

using Godot;
using System;

public partial class Home : Control
{
    private PlayerStateManager CurrentPlayer { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        CurrentPlayer = PlayerStateManager.Instance;
        
        if (CurrentPlayer.token == null)
        {
            
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_battle_button_pressed()
	{
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.INGAMEMODE);
	}
}
