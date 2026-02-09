namespace FSDClient.builder;

using FSDClient.view;
using FSDClient.data;
using Godot;

public record CardViewData(string Color, string IconName, int Cost, int Health, int Attack);

public class CardBuilder
{
    public static CardView Build(CardData CardData)
    {
        var data = new CardViewData(CardData.Color.ToString().ToLower(), CardData.Name, CardData.Cost, CardData.Health, CardData.Attack);
        var CardDisplay = new CardView();
        CardDisplay.Setup(data);
        return CardDisplay;
    }

}
