using FSDClient.builder;
using Godot;
using FSDClient.player;
using FSDClient.player.display;
using FSDClient.card.display;
using FSDClient.card;
using FSDClient.battlefield.handManagement;
using System;

namespace FSDClient.battlefield;


public partial class Gameloop : Node2D
{

	public static readonly double MAX_ELIXER = 8;
	public static readonly double ROUND_TIMER = 30;
	public static readonly double SECONDS_PER_ELIXIR = 5f;
	public static readonly int BASE_ELIXIR = 5;


	public NetworkManager NetworkManager { get; set; }
	// Unsure to keep this as the state manager or make it it's own individual player data. TBD on a later date
	public PlayerStateManager MainPlayer { get; set; }
	// This info, leave majority null, the info is not needed so extensively
	// TODO: Get rid of some fields and move the PlayerStateManager as that is what needs to
	// Get rid of this can be read elsewhere 
	public PlayerData IncomingPlayer { get; set; }
	
	// Deal with card placement and card movement 
	private Card[][] Board { get; set; } = new Card[4][];
	private CardManager CardManager;
	
	// Parameters for game state to be managed
	private int Elixir { get; set; }
	private int RoundNumber { get; set; }
	private double GameTimer { get; set; } = 0;
	private double RegenInterval { get; set; } = 1;
	private int TurnRound = 1;

	// public void StartGameLoop()
	// {
	//     // TODO: Should be obvious
	//     while (true)
	//     {

	//         break;
	//     }

	//     ReturnToHomeScreen();

	// }

	// I am of the assumption that this is what is being called by the 
	// We put this in gameloop later
	public override void _Ready()
	{

		for (int i = 0; i < 4; i++)
		{
			Board[i] = new Card[3];
		}

		MainPlayer = PlayerStateManager.Instance;
		// TODO: Figure out how network protocol is set up then replace data with incoming information from
		//       network
		IncomingPlayer = new PlayerData("Placeholder", "Placeholder", [], false);
		
		var TestCard = new CardData(10, "robot", Colour.RED, 100, 10);
		var CardTexture = Builder.BuildCard(TestCard);

		// // Board[0][0] = new Card();
		// // Board[0][0].InitializeCard(CardView);
		// // AddChild(Board[0][0]);

		// This is a valid example of how to load a new card
		// var CardManager = (CardManager)FindChild("CardManager", true);
		// var CardScene = GD.Load<PackedScene>("res://scenes/Card.tscn");
		// var CardTemp = CardScene.Instantiate<Card>();
		// CardTemp.LoadDataTexture(CardTexture);
		// CardManager.AddChild(CardTemp);

		// This is just to test whether it would load
		// SINCE THIS IS NOT TO BE CONSTANTLY DESTROYED AND RECREATED, THIS IS A POC TO TEST IF THE 
		// THING WILL LOAD (sorry Jared)
		GD.Print("Attempting to create the initial player view");
		try {
			var PlayerTextureView = Builder.BuildPlayer();
			// var PlayerIcon = (PlayerView)FindChild("PlayerIcon");
			var PlayerTrial = GD.Load<PackedScene>("res://scenes/PlayerIcon.tscn");
			var PlayerIcon = PlayerTrial.Instantiate<PlayerView>();
			PlayerIcon.LoadDataTexture(PlayerTextureView);
			PlayerIcon.Scale = new Vector2(0.35f, 0.35f);
			AddChild(PlayerIcon);
			
		} catch (Exception e) {
			GD.PrintErr("Exception: ", e);
		}

        GD.Print("Successfully created the initial player view");
		
        // See how to initialise Elixir 

	}
	// This is how to generate Elixir. Since client only ever knows about 1 player's resource
	// i can do it within the gameplay loop itself
	public override void _Process(double delta)
	{
		// GD.Print("Called");
		if (Elixir >= MAX_ELIXER && Elixir > TurnRound + BASE_ELIXIR)
			return;

		RegenInterval += delta;

		if (RegenInterval >= SECONDS_PER_ELIXIR)
		{
			Elixir++;
			RegenInterval = 0f;
			var ElixirBar = (Elixir)FindChild("Elixir");
			ElixirBar.UpdateElixir(Elixir);
		}

		if (GameTimer > ROUND_TIMER * TurnRound)
		{
			TurnRound += 1;
			// TODO: Trigger secondary draw card event
			
		}
		GameTimer += delta;

	}

	public bool PlaceCardCheck(int Cost)
	{
		if (Elixir < Cost)
		{
			return false;
		}

		Elixir -= Cost;

		return true;
	}

	/*
		TODO: Fix this later
	*/
	public void PlaceCardInSlot(CardData cardData, int xPos, int yPos)
	{

		if (yPos < 0 || yPos > Board.Length || xPos < 0 || xPos > Board[yPos].Length || Board[yPos][xPos] != null)
		{
			// Throw some sort of exception that throws back the card
			// into the hand visually
			return;
		}
		Board[yPos][xPos] = new();
	}

	/*
	* This function is to explicitely listen to server's response from the network
	* Assuming how we wish to implement the Elixer tracking, we can do via
	*   -> Player attempts to play card. One check on clientside and one check
	*      server side in order to see weather valid therefore needs to manage the
	*      result from this
	*   -> Player 2 display a card. Since it is only a 3 by 2 2d array, this shouldnt be
	*      too hard to store
	*   -> Are we holding client and server timer for card attacks as well?
	*/
	public static void listenForServerReaction()
	{

	}

	/*
	* I am going on a whim here but this should be called within the main game loop
	*/
	public static void writeToServer()
	{

	}

	private void ReturnToHomeScreen()
	{
		GameStateManager.Instance.ChangeGameState(GameState.HOMESCREEN);
	}

}
