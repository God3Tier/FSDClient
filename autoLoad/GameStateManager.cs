using Godot;

namespace Client.autoLoad;

enum GameState
{
    HOMESCREEN,
    DECKSELECTION,
    SEARCHINGFORPLAYER,
    INGAMEMODE
}

public partial class GameStateManager : Node
{

    private GameState CurrentGameState { get; set; }

    public override void _Ready()
    {
        switch (CurrentGameState)
        {
            case GameState.DECKSELECTION:
                {
                    break;
                }
            case GameState.HOMESCREEN:
                {
                    break;
                }
            case GameState.SEARCHINGFORPLAYER:
                {
                    break;
                }
            case GameState.INGAMEMODE:
                {
                    break;
                }
        }
    }

    public override void _Process(double delta)
    {

    }

}
