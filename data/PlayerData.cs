using System.Collections;
using System.Collections.Generic;
using Godot;

namespace Client.data;

public class PlayerData
{
    private string Username { get; set; }
    private int Elixer { get; set; } = 0;
    private string IconName { get; set; }
    private Sprite2D Sprite2D { get; } = new();
    private int Health { get; set; } = 250;
    private int Attack { get; } = 5;
    private List<CardData> CurrHand { get; } = new();
    private List<CardData> DeckCardDatas { get; }

    public PlayerData(string username, string iconName, List<CardData> deckCardDatas)
    {
        Username = username;
        IconName = iconName;
        DeckCardDatas = deckCardDatas;
        // TODO change this to the actual directory
        var texture2D = ResourceLoader.Load<Texture2D>(iconName);
        Sprite2D.Texture = texture2D;
    }


}
