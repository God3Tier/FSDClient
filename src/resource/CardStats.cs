namespace FSDClient.resource;

using Godot;

[GlobalClass]
public partial class CardStats : Resource
{
	[Export]
	public int id;
	
	[Export]
	public Texture2D Image;
	
	[Export]
	public string Name;
	
	[Export]
	public string Colour;
	
	[Export]
	public string Rarity;
	
	[Export]
	public int Cost;
	
	[Export]
	public int Attack;
	
	[Export]
	public int Health;
	
	[Export]
	public string Effect;
	
	[Export]
	public string Summon;
	
	[Export]
	public string OnAttack;
	
	[Export]
	public string OnDamaged;
	
	[Export]
	public string OnDeath;

	// This is apparently a necessity according to the docs to load the information
	public CardStats() : this(0, "res://assets/characters/Farmer.png", "Farmer", "Grey", "common", 2, 10, 10, "No effect", "basic", "basic", "basic", "basic") { }

	public CardStats(int id, string imagepath, string name, string colour, string rarity, int cost, int attack, int health, string effect, string summon, string onattack, string ondamaged, string ondeath) 
	{
		this.id = id;
		this.Image = GD.Load<Texture2D>(imagepath);
		this.Name = name;
		this.Colour = colour;
		this.Rarity = rarity;
		this.Cost = cost;
		this.Attack = attack;
		this.Health = health;
		this.Effect = effect;
		this.Summon = summon;
		this.OnAttack = onattack;
		this.OnDamaged = ondamaged;
		this.OnDeath = ondeath;
	}
}
