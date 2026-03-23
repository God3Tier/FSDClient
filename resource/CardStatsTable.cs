namespace FSDClient.resource;

using Godot;
public partial class CardStatsTable : Resource
{
    public Godot.Collections.Dictionary<int, CardStats> cardInfo = new();
    
    public CardStatsTable () {
        // Load from some stupid csv or something idk 
    }
}
