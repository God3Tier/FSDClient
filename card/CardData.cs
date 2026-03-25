namespace FSDClient.card;

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
	public double TimeToAttack { get; }


	public CardData(int cost, string name, Colour colour, int health, int attack, double timeToAttack)
	{
		Cost = cost;
		Name = name.ToLower();
		Color = colour;
		Health = health;
		Attack = attack;
		TimeToAttack = timeToAttack;
		// Set url of this later
	}

}
