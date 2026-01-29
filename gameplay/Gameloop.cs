using System;
using System.Collections.Generic;
using System.Threading;
using Client.autoLoad;
using Client.data;
using Godot;
using static Godot.GD;

namespace Client.gameplay;


public partial class Gameloop: Node2D
{

    public NetworkManager NetworkManager { get; set; }
    public PlayerData PlayerData1 { get; set; }
    public PlayerData PlayerData2 { get; set; }
    // Haha funny math
    private CardData[][] Board { get; set; } = new CardData[4][];


    public void StartGameLoop()
    {
        
    }
    
    // I am of the assumption that this is what is being called by the 
    public override void _Ready()
    {

        // This is just for me to test out my threading implementation of the program
        List<CardData> cardDatas = [new(10, "Hello"), new(15, "Goodbye")];
        PlayerData pd1 = new("John", "Gay icon", cardDatas);
    
        Thread.Sleep(8000);
        Print("pd1 Elixer " + pd1.Elixer);
        pd1.EndGame();

        pd1.SyncRemoveElixer(3); 
        Print("pd1 Elixer after removing 3 " + pd1.Elixer); 
        
        
    }

    public override void _Process(double delta)
    {
        
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

}
