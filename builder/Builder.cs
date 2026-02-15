namespace FSDClient.builder;

using FSDClient.view;
using FSDClient.data;

/*
    This is a static class that contains the 2 builder methods to construct the control (NOT NODE) of the card. 
    This is to be used within the scene to create the card / player icon onto the screen. 
*/
public class Builder
{
    // This constructs the card from the CardData class 
    public static CardView BuildCard(CardData CardData)
    {
        var data = new CardViewData(CardData.Color.ToString().ToLower(), CardData.Name, CardData.Cost, CardData.Health, CardData.Attack);
        var CardViewTextures = new CardViewTextures(data);
        var CardDisplay = new CardView(CardViewTextures);
        return CardDisplay;
    }

    // Have not figured out how I wish to parse data to it. Will use placeholders for the time being 
    // TODO: Look up 
    public static PlayerView BuildPlayer()
    {
        var data = new PlayerViewData("purple","purple", 100, 5);
        var PlayerViewTextures = new PlayerViewTextures(data);
        var PlayerDisplay = new PlayerView(PlayerViewTextures);
        return PlayerDisplay;
    }
}