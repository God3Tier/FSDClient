using System.Collections.Generic;
using Godot;

using FSDClient.player;
using FSDClient.card;


/*
    Upon Client being opened, it is expected to have all user data instantly, hence it is an autoloaded class. 
    It has an instance pointer of itself, the PlayerGame data and the List of cards it has access to. 
    I am going to add more things as the other scenes get created as I am not sure what other information 
    we need the user to have. 
*/
public partial class PlayerStateManager : Node
{
    public static PlayerStateManager Instance { get; private set; }
    public PlayerData PlayerData { get; private set; }
    private List<CardData> DeckCardDatas { get; set; }

    public override void _Ready()
    {
        // Note this is a placeholder until I know what protocol we will use to load player data into the network
        DeckCardDatas = new();
        PlayerData = new PlayerData("Placeholder", "Placeholder", DeckCardDatas, true);
        Instance = this;
    }

    // Here, we should request however the server stores the data of the user
    public void LoadIntoData()
    {

    }


}
