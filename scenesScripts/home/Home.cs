namespace FSDClient.home;

using Godot;
using System;
using FSDClient.autoLoad;

public partial class Home : Control
{
    private PlayerStateManager CurrentPlayer { get; set; }
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
                GameStateManager.ChangeGameState(GameState.LOGIN);
            }

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

    public void _on_battle_button_pressed()
    {
        var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
        GameStateManager.ChangeGameState(GameState.INGAMEMODE);
    }

    public void _on_card_button_pressed()
    {
        var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
        GameStateManager.ChangeGameState(GameState.CARDSCREEN);
    }
}
