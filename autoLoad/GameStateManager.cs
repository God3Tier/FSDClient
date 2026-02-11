using Godot;

/*
    These are the different Gamestates that can exist for the application. The player is always either in
    one of these scenes
*/
public enum GameState
{
    HOMESCREEN,
    DECKSELECTION,
    SEARCHINGFORPLAYER,
    INGAMEMODE
}

/*
    This is an autoloaded Manager that has a static instance that is a reference to the class. It is to be instantiated 
    at the start of the client being open. This manages at which scene the main node will be running and is to be called 
    to switch between scenes
*/
public partial class GameStateManager : Node
{

	private Node CurrentScene { get; set; }
	private Node SceneContainer { get; set; }
	
	public static GameStateManager Instance { get; private set; }

	// This will be run when the class is constrcuted. It will get the SceneContainer and will initialise to the 
	// homescreen state
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
