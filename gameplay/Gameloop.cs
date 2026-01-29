using Client.autoLoad;
using Client.data;

namespace Client.gameplay;


public class Gameplay(PlayerData playerData1, PlayerData playerData2)
{

    private NetworkManager NetworkManager;
    private PlayerData PlayerData1 = playerData1;
    private PlayerData PlayerData2 = playerData2;
    // Haha funny math
    private CardData[][] Board = new CardData[4][];


    public void startGameLoop()
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
