using Godot;
using System;

public partial class Card : Sprite2D
{	
	[Signal] public delegate void HoveredEventHandler(Card card);
	[Signal] public delegate void HoveredOffEventHandler(Card card);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var CardManager = GetParent<CardManager>();
		if (CardManager != null)
		{
			Hovered += CardManager.OnHoverOverCard;
			HoveredOff += CardManager.OnHoverOffCard;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnMouseEntered()
	{
		EmitSignal(SignalName.Hovered, this);
	}
	
	public void OnMouseExited()
	{
		EmitSignal(SignalName.HoveredOff, this);
	}
	
}
