namespace FSDClient.resource;

using Godot;

[GlobalClass]
public partial class CardStats : Resource
{
	[Export]
	public int CardId;
	
	[Export]
	public int Health;

	[Export]
	public int Attack;

	[Export]
	public string CardColour;

	[Export]
	public string IconName;

	[Export]
	public int Cost;

	[Export]
	public string Name;

	[Export]
	public int TimeToAttack;

	// This is apparently a necessity according to the docs to load the information
	public CardStats() : this(0, 0, null, null, 0, null, 0) { }

	public CardStats(int health, int attack, string cardColour, string iconName, int cost, string name, int timeToAttack)
	{
		Health = health;
		Attack = attack;
		CardColour = cardColour;
		IconName = iconName;
		Cost = cost;
		Name = name;
		TimeToAttack = timeToAttack;
	}
}
