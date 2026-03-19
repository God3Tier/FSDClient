namespace FSDClient.resource;

using Godot;

[GlobalClass]
public partial class CardStats : Resource
{
    [Export]
    public int Health;

    [Export]
    public int Attack;

    [Export]
    public string CardColour;

    [Export]
    public string IconName;

    // This is apparently a necessity according to the docs to load the information
    public CardStats() : this(0, 0, null, null) { }

    public CardStats(int health, int attack, string cardColour, string iconName)
    {
        Health = health;
        Attack = attack;
        CardColour = cardColour;
        IconName = iconName;
    }
}
