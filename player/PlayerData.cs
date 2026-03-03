namespace FSDClient.player;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using FSDClient.card;

public class PlayerData
{
    private string Username { get; set; }
    
    // TODO: Not sure whether this is standard to keep it individual or to create an abstracted class
    //       that acts as a wrapper around the data but ts looks ugly
    private string IconName { get; set; }
    private bool MainPlayer { get; set; }
    // Numbers to be confirmed. Will place a proper named thing once everything has been sorted out 
    private int Health { get; set; } = 250;
    private int Attack { get; } = 5;
    public List<CardData> CurrHand { get; } = new();
    private List<CardData> DeckCardDatas { get; }

    public PlayerData(string username, string iconName, List<CardData> deckCardDatas, bool mainPlayer)
    {
        Username = username;
        IconName = iconName;
        DeckCardDatas = deckCardDatas;
        MainPlayer = mainPlayer;
    }
}
