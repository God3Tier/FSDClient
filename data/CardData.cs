namespace FSDClient.data;

using Godot;

public class CardData
{
    // Ts is not exaustive yet. I need to see whether i need a timer here. But 
    // How to sync with server? 
    private int Cost { get; }
    private string Name { get; }
    private Colour Color { get; set;}
    private int Health { get; set; }
    private int Attack { get; }


    public CardData(int cost, string name, Colour colour)
    {
        Cost = cost;
        Name = name;
        Color = colour;
        // Set url of this later
    }
    
}
