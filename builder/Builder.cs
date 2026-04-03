namespace FSDClient.builder;

using FSDClient.card.display;
using FSDClient.card;
using FSDClient.player.display;
using FSDClient.player;

using FSDClient.resource;

/*
    This is a static class that contains the 2 builder methods to construct the control (NOT NODE) of the card. 
    This is to be used within the scene to create the card / player icon onto the screen. 
*/
// This class is to be depricated
public class Builder
{
    private static readonly int TIME_TO_ATTACK = 10;
	// This constructs the card from the CardData class 
	public static CardViewTextures BuildCard(CardData CardData)
	{
		var data = new CardViewData(CardData.Color.ToString().ToLower(), CardData.Name, CardData.Cost, CardData.Health, CardData.Attack, CardData.TimeToAttack);
		return new CardViewTextures(data);
	}
	
	public static CardViewTextures BuildCard(CardStats CardStats)
	{
		var data = new CardViewData(CardStats.Colour.ToLower(), CardStats.Name, CardStats.Cost, CardStats.Health, CardStats.Attack, TIME_TO_ATTACK);
		return new CardViewTextures(data);
	}

	// Have not figured out how I wish to parse data to it. Will use placeholders for the time being 
	// TODO: Look up 
	public static PlayerViewTextures BuildPlayer(PlayerData RawPlayerData)
	{
		var data = new PlayerViewData(RawPlayerData.Username, RawPlayerData.IconName, RawPlayerData.BorderColour, 100, 5);
		// var data = new PlayerViewData("temp", "purple", "purple", 100, 5);
		var PlayerViewTextures = new PlayerViewTextures(data);
		return PlayerViewTextures;
	}
}