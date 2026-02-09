using System.Collections.Generic;
using FSDClient.data;
using Godot;


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
