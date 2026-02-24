namespace FSDClient.card.display;

using Godot;
using System;
using FSDClient.battlefield.handManagement;

public partial class Card : Node2D
{
	[Signal] public delegate void HoveredEventHandler(Card card);
	[Signal] public delegate void HoveredOffEventHandler(Card card);
	public Vector2 StartingPosition;
	// hi

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// GD.Print("Ready successfully called");
		try
		{
			var CardManager = GetParent<CardManager>();
			if (CardManager != null)
			{
				Hovered += CardManager.OnHoverOverCard;
				HoveredOff += CardManager.OnHoverOffCard;
			}
			GD.Print("Successfully initialised Parent Card Manager");

			var area = (Area2D)FindChild("Area2D", true);
			area.MouseEntered += OnMouseEntered;
			area.MouseExited += OnMouseExited;

			// var control = FindChild("CardView", true);


		}
		catch (Exception e)
		{
			GD.PrintErr("Whoops ", e);
		}



	}

	// public void InitializeCard(CardView CardView)
	// {

	//     var control = FindChild("CardView", true);
	//     AddChild(CardView);
	//     GD.Print("Initialized the card into the Card");
	// }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void LoadDataTexture(CardViewTextures cardViewTextures)
	{
		var BorderTexture = (TextureRect)FindChild("Border", true);
		BorderTexture.Texture = cardViewTextures.BorderTexture;
		var IconTexture = (TextureRect)FindChild("Icon", true);
		IconTexture.Texture = cardViewTextures.IconTexture;
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
