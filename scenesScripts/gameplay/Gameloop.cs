using System;
using System.Collections.Generic;
using System.Threading;
using FSDClient.data;
using Godot;
using static Godot.GD;

namespace FSDClient.gameplay;


public partial class Gameloop: Node2D
{

	public NetworkManager NetworkManager { get; set; }
	// Unsure to keep this as the state manager or make it it's own individual player data. TBD on a later date
	public PlayerStateManager MainPlayer { get; set; }
	// This info, leave majority null, the info is not needed so extensively
	// TODO: Get rid of some fields and move the PlayerStateManager as that is what needs to 
	public PlayerData IncomingPlayer { get; set; }
	private int RoundNumber { get; set; }
	// Haha funny math
	private CardData[][] Board { get; set; } = new CardData[4][];

	public void StartGameLoop()
	{
		// TODO: Should be obvious
		while (true)
		{
			
			break;
		}

		ReturnToHomeScreen();

	}

	// I am of the assumption that this is what is being called by the
	public override void _Ready()
	{

		for (int i = 0; i < 4; i++)
		{
			Board[i] = new CardData[3];
		}

		MainPlayer = PlayerStateManager.Instance;
		// TODO: Figure out how network protocol is set up then replace data with incoming information from 
		//       network
		IncomingPlayer = new PlayerData("Placeholder", "Placeholder", [], false);

		// This is just for me to test out my threading implementation of the program
		// List<CardData> cardDatas = [new(10, "Hello"), new(15, "Goodbye")];
		// PlayerData pd1 = new("John", "Gay icon", cardDatas);

		// Thread.Sleep(8000);
		// Print("pd1 Elixer " + pd1.Elixer);
		// pd1.EndGame();

		// pd1.SyncRemoveElixer(3);
		// Print("pd1 Elixer after removing 3 " + pd1.Elixer);


	}
	// TODO: This I assume is how rendering + input control is supposed to take place. I am in the process of loading all the necessary
	//       component to this script's .tscn. This is a lower priority since console debugging is for sure more efficient right :-)
	public override void _Process(double delta)
	{
		
	}


	public void PlaceCardInSlot(CardData cardData, int xPos, int yPos)
	{

		if (yPos < 0 || yPos > Board.Length || xPos < 0 || xPos > Board[yPos].Length || Board[yPos][xPos] != null)
		{
			// Throw some sort of exception that throws back the card 
			// into the hand visually
			return;
		}
		Board[yPos][xPos] = cardData;
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
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.HOMESCREEN);
	}

}
