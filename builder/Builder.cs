namespace FSDClient.builder;

using FSDClient.view;
using FSDClient.data;
using Godot;
public record PlayerViewData(string IconName, string Color, int Health, int Attack);

public class Builder
{
    // These 2 cam be abstracted to one class. Not sure whether it makes sense for this context 
    public static CardView BuildCard(CardData CardData)
    {
        var data = new CardViewData(CardData.Color.ToString().ToLower(), CardData.Name, CardData.Cost, CardData.Health, CardData.Attack);
        var CardViewTextures = new CardViewTextures(data);
        var CardDisplay = new CardView(CardViewTextures);
        // CardDisplay.Setup(data);
        return CardDisplay;
    }
    
    // Have not figured out how I wish to parse data to it. Will use placeholders for the time being 
    // TODO: Look up 
    public static PlayerView BuildPlayer()
    {
        var data = new PlayerViewData("grey", "grey", 100, 5);
        var PlayerDisplay = new PlayerView(data);
        return PlayerDisplay;
    }
}
