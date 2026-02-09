namespace FSDClient.data;

using Godot;

public enum Colour
{
    RED,
    BLUE,
    GREEN,
    YELLOW,
    PURPLE,
    GREY,
}

public class CardData
{
    // Ts is not exaustive yet. I need to see whether i need a timer here. But 
    // How to sync with server? 
    public int Cost { get; }
    public string Name { get; }
    public Colour Color { get; set; }
    public int Health { get; private set; }
    public int Attack { get; }


    public CardData(int cost, string name, Colour colour, int health, int attack)
    {
        Cost = cost;
        Name = name.ToLower();
        Color = colour;
        Health = health;
        Attack = attack;
        // Set url of this later
    }

}
