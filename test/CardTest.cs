namespace FSDClient.test;

using Godot;
using FSDClient.data;
using FSDClient.builder;

/*
	This class is to test everything related to card. It should contain
	the script to test it's ability to receive damagge and render image
	Will think of more things this is supposed when the time comes 
*/

public partial class CardTest : Node2D
{
	private CardData TestCard {get; set;}
	[Export] private PackedScene CardViewTest; 
	
	public override void _Ready() {
		
		TestCard = new CardData(10, "robot", Colour.RED, 100, 10); 
		CardBuilder cardBuilder = new();
		var CardView = cardBuilder.Build(TestCard);
		AddChild(CardView);
	}
	
	
	public override void _Process(double delta)
	{
		
		
	}
}
