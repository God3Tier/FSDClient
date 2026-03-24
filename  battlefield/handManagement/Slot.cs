namespace FSDClient.battlefield.handManagement;

using Godot;
using System;
using FSDClient.card.display;

public partial class Slot : Node2D
{
	public bool CardInSlot = false;
	public Card Card { get; set;}
	
	public void AddCard(Card card) {
		this.CardInSlot = true;
		this.Card = card;
	}
	
	public void RemoveCard() {
		this.CardInSlot = false;
		this.Card = null;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
