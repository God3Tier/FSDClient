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

	// Calculate the total stat at that level
	public static int LevelStatsCalculator(int BaseStat, int Level)
	{
		// for it to be level 1 stat * 1.2 (for each level)
		int LeveledStats = (int)Math.Ceiling( BaseStat * (1 + (0.2 * (Level - 1))));
		
		return LeveledStats;
	}

	// Calculate the card cost to get to the next level
	public static int CardCostCalculator(int Level)
	{
		// for it to be 4 ^ level
		return (int)Math.Pow(4, Level);
	}

	// Calculate the crystal cost to get to the next level
	public static int CrystalCostCalculator(int Level)
	{
		// for it to be 100 ^ level
		return (int)Math.Pow(100, Level);
	}

}
