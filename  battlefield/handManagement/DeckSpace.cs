namespace FSDClient.battlefield.handManagement;

using Godot;
using System;
using FSDClient.card.display;


public partial class DeckSpace : HandControl
{
	[Signal]
	public delegate void RemoveCardMessageEventHandler(int cardID);
	public bool SuppressSignals { get; set; } = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this._cardLimit = 8;
		this._cardList = new Card[_cardLimit];
		this._slotList = new DeckSlot[_cardLimit];
		foreach (Node child in GetChildren())
		{
			string childName = child.Name.ToString();
			DeckSlot slot = (DeckSlot)child;
			int lastInt = (int)(childName[^1] - '1');
			_slotList[lastInt] = slot;
		}

		base._Ready();

	}

	public override bool AddCard(Card card, Slot slot)
	{
		GD.Print("Removing card from current deck");
		if (base.AddCard(card, slot) && !SuppressSignals)
		{
			EmitSignal(SignalName.RemoveCardMessage, card.CardID);

			return true;
		}

		return false;
	}
	public override bool AddCard(Card card)
	{
		GD.Print("Removing card from current deck");
		if (base.AddCard(card) && !SuppressSignals)
		{
			EmitSignal(SignalName.RemoveCardMessage, card.CardID);
			return true;
		}

		return false;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
