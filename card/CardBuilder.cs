namespace FSDClient.card;

using Godot;
using System;

using FSDClient.builder;
using FSDClient.resource;
using FSDClient.autoLoad;
using FSDClient.card.display;
using System.Collections.Generic;

public class CardBuilder
{
    private static CardStatsTable Reference =>
    ((SceneTree)Engine.GetMainLoop()).Root
        .GetNode<ResourceManager>("/root/ResourceManager")
        .CardStatsTable;
    private static readonly PackedScene packedScene = ResourceLoader.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");



    public static Card GenerateCard(int cardID)
    {
        var stats = Reference.cardInfo[cardID];
        var textures = Builder.BuildCard(stats);
        var CardScene = GD.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");
        var CardTemp = CardScene.Instantiate<Card>();
        CardTemp.LoadDataTexture(textures);
        return CardTemp;
    }

    public static void LoadTextureFromId(int cardID, Card card)
    {
        var stats = Reference.cardInfo[cardID];
        var textures = Builder.BuildCard(stats);
        card.LoadDataTexture(textures);
    }

    // Get card values (specifically for cardmanagement page)
    public static CardStats GetCardValues(int cardID)
	{
		CardStats stats = Reference.cardInfo[cardID];
		return stats;
	}
}
