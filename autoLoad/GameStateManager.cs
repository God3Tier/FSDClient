using Godot;

public enum GameState
{
	HOMESCREEN,
	DECKSELECTION,
	SEARCHINGFORPLAYER,
	INGAMEMODE
}

public partial class GameStateManager : Node
{

	private Node CurrentScene { get; set; }
	private Node SceneContainer { get; set; }
	
	public static GameStateManager Instance { get; private set; }

	// Here, we initialize the gamestate 
	public override void _Ready()
	{
		var sceneContainer = GetNode<Node>("SceneContainer");
		SceneContainer = sceneContainer;
		ChangeGameState(GameState.HOMESCREEN);

		Instance = this;
	}

	// When switching gamestate, use the enum and they are suppose to change
	// to the appropriate scene. After that, nuke everything. Currently I havent added 
	// anything cause I want to merge to main when I create the dummy nodes. 
	public void ChangeGameState(GameState gameState)
	{
		switch (gameState)
		{
			case GameState.HOMESCREEN:
				{
					ChangeScene("res://scenes/states/home.tscn");
					break;
				}
			case GameState.DECKSELECTION:
				{
					break;
				}

			case GameState.INGAMEMODE:
				{
					break;
				}
			case GameState.SEARCHINGFORPLAYER:
				{
					break;
				}
		}
	}

   
	// This function is to change between the .tscn files. It first gets it as a PackedScene
	// Then Initiates it into a usable node. It is then appended to a queue that will wait 
	// for the process/ ready to be finished before freeing the memory
	public void ChangeScene(string path)
	{
		SceneContainer?.QueueFree();
		var packedScene = GetNode<PackedScene>(path);
		CurrentScene = packedScene.Instantiate();
		SceneContainer.AddChild(CurrentScene);
	}

}
