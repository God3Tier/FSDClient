namespace FSDClient.player;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using FSDClient.card;

public class PlayerData
{
    public string Username { get; set; }
    
    // TODO: Not sure whether this is standard to keep it individual or to create an abstracted class
    //       that acts as a wrapper around the data but ts looks ugly
    public string IconName { get; set; }
    public string BorderColour { get; set; }
    public bool MainPlayer { get; set; }
    
    // IDK if these 2 is necessary. Once I do more of the game logic I will figure it out
    public List<CardData> CurrHand { get; } = new();
    public List<CardData> DeckCardDatas { get; }

    public PlayerData(string username, string iconName, string borderColour,  List<CardData> deckCardDatas, bool mainPlayer)
    {
        Username = username;
        IconName = iconName;
        DeckCardDatas = deckCardDatas;
        MainPlayer = mainPlayer;
        BorderColour = borderColour;
    }
}
