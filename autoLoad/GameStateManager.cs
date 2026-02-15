using Godot;
using System.Collections.Generic;

/*
	This is an autoloaded Manager that has a static instance that is a reference to the class. It is to be instantiated 
	at the start of the client being open. This manages at which scene the main node will be running and is to be called 
	to switch between scenes
*/

	/*
	These are the different Gamestates that can exist for the application. The player is always either in
	one of these scenes
	
	Need to also update the dictionary from the class to map it to paths
*/
public enum GameState
{
	HOMESCREEN,
	INGAMEMODE
}

public partial class GameStateManager : Node
{

	private Node CurrentScene { get; set; }
	private Node SceneContainer { get; set; }
	public GameState CurrentGameState { get; private set; }
	
	public static GameStateManager Instance { get; private set; }

	/*
	Update the dictionary mapping here
*/

	private readonly Dictionary<GameState, string> _stateToScene = new()
	{
		{ GameState.HOMESCREEN, "res://scenes/states/home.tscn" },
		{ GameState.INGAMEMODE, "res://scenes/states/gameplay.tscn" }
	};

	// Here, we initialize the gamestate 
	public override void _Ready()
	{
		//var sceneContainer = GetNode<Node>("SceneContainer");
		//SceneContainer = sceneContainer;
		//ChangeGameState(GameState.HOMESCREEN);
//
		//Instance = this;
		
		Viewport root = GetTree().Root;
		// Using a negative index counts from the end, so this gets the last child node of `root`.
		CurrentScene = root.GetChild(-1);
	}

	// When switching gamestate, use the enum and they are suppose to change
	// to the appropriate scene. After that, nuke everything. Currently I havent added 
	// anything cause I want to merge to main when I create the dummy nodes. 
	public void ChangeGameState(GameState gameState)
	{
		CurrentGameState = gameState;

		if (_stateToScene.TryGetValue(gameState, out var path))
		{
			GotoScene(path);
		}
	}

   
	private void GotoScene(string path)
	{
		CallDeferred(MethodName.DeferredGotoScene, path);
	}

	public void DeferredGotoScene(string path)
	{
		// It is now safe to remove the current scene.
		CurrentScene.Free();

		// Load a new scene.
		var nextScene = GD.Load<PackedScene>(path);

		// Instance the new scene.
		CurrentScene = nextScene.Instantiate();

		// Add it to the active scene, as child of root.
		GetTree().Root.AddChild(CurrentScene);

		// Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
		GetTree().CurrentScene = CurrentScene;
	}

}
