namespace FSDClient.card.display;

using Godot;
using System;
using FSDClient.battlefield.handManagement;
using FSDClient.card.mechanics.effects;

public partial class Card : Node2D
{
	[Signal] public delegate void HoveredEventHandler(Card card);
	[Signal] public delegate void HoveredOffEventHandler(Card card);
	[Signal] public delegate void AttackedEventHandler(Card card);

	public Vector2 StartingPosition { get; set; }
	public Tween MoveTween { get; set; }
	public int CardID { get; set; }
	public int Health { get; set; }
	public int Attack { get; set; }
	public string Colour { get; set; }
	private bool BattleMode { get; set; } = false;
	public double TimeToAttack { get; set; }
	public double Timer { get; set; }
	public int ActiveY { get; set; }
	public int ActiveX { get; set; }
	public bool IsEmpty { get; set; } = true;

	public enum SlotStatus
	{
		Pack,
		Deck,
		HandTemp,
		HandPause,
		Hand,
		Battle
	}

	public SlotStatus CurrentSlotStatus { get; set; }
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
				var area = (Area2D)FindChild("Area2D", true);
				area.MouseEntered += OnMouseEntered;
				area.MouseExited += OnMouseExited;
			}
			GD.Print("Successfully initialised Parent Card Manager");
		}
		catch (Exception e)
		{
			var Area2D = FindChild("Area2D");
			Area2D.QueueFree();
			// GD.Print("This is an opponent card");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!BattleMode)
		{
			return;
		}


		if (Timer + delta >= TimeToAttack)
		{
			Timer = 0;
			EmitSignal(SignalName.Attacked, this);
			return;
		}

		Timer += delta;
		var ProgressBar = (ProgressBar)FindChild("ProgressBar");
		ProgressBar.Value = Timer;
	}

	public void LoadDataTexture(CardViewTextures cardViewTextures)
	{
		GD.Print("Creating card");

		CardID = cardViewTextures.CardID;

		var BorderTexture = (Sprite2D)FindChild("Border", true);
		BorderTexture.Texture = cardViewTextures.BorderTexture;
		BorderTexture.Scale = new Vector2(0.500f, 0.500f);

		var IconTexture = (Sprite2D)FindChild("Icon", true);
		IconTexture.Texture = cardViewTextures.IconTexture;
		// IconTexture.Scale = new Vector2(0.468f, 0.423f);

		var AttackValue = (RichTextLabel)FindChild("Attack", true);
		AttackValue.Text = cardViewTextures.AttackValue;
		if (int.TryParse(cardViewTextures.AttackValue, out int attack))
		{
			Attack = attack;
		}

		var CurrentHealth = (RichTextLabel)FindChild("Health", true);
		CurrentHealth.Text = cardViewTextures.CurrentHealth;
		if (int.TryParse(cardViewTextures.CurrentHealth, out int currentHealth))
		{
			Health = currentHealth;
		}

		var ElixirCost = (RichTextLabel)FindChild("ElixirCost", true);
		ElixirCost.Text = cardViewTextures.ElixirCost;

		var ProgressBar = (ProgressBar)FindChild("ProgressBar", true);
		ProgressBar.MaxValue = cardViewTextures.TimeToAttack;
		TimeToAttack = cardViewTextures.TimeToAttack;

		GD.Print("Able to create card");
	}

	// To update the stats of the current card
	// damage must be negative, else it will be treated as a heal
	// returns true if the card is dead
	public bool UpdateStats(int attackChange, int damage)
	{
		if (attackChange != 0) // change attack
		{
			// Get the attack label of the card
			var CurrentAttack = (RichTextLabel)FindChild("Attack", true);
			Attack += attackChange;
			CurrentAttack.Text = Attack.ToString();
		}
		if (damage != 0) // change health
		{
			// Get the attack label of the card
			var CurrentHealth = (RichTextLabel)FindChild("Health", true);
			Health += damage;
			CurrentHealth.Text = Health.ToString();
			
			if (Health <= 0)
			{
				return true;
			}
		}
		return false;
	}
	
	// Render card into battleslot
	public void EnterBattleSlot()
	{
		var ElixirCost = (RichTextLabel)FindChild("ElixirCost");
		ElixirCost.Text = "";
		CurrentSlotStatus = SlotStatus.Battle;
		IsEmpty = false;
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
