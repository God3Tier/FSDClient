using Godot;
using System;
using FSDClient.card.display;
namespace FSDClient.battlefield.handManagement;

public partial class BattleSlot : Node2D
{
	// TODO: Remove to check if card is null 
	public bool CardInSlot = false;
	public int x { get; set; }
	public int y { get; set; }
	public Card Card { get; set;}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
