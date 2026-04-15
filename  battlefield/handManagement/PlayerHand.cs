namespace FSDClient.battlefield.handManagement;

using Godot;
using System;
using System.Linq;
using FSDClient.card.display;


public partial class PlayerHand : HandControl
{	
	[Signal]
	public delegate void AddCardMessageEventHandler(int cardID);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this._cardLimit = 4;
		this._cardList = new Card[_cardLimit];
		this._slotList = new HandSlot[_cardLimit];

		foreach (Node child in GetChildren())
		{
			string childName = child.Name.ToString();
			HandSlot slot = (HandSlot)child;
			int lastInt = (int)(childName[^1] - '1');
			_slotList[lastInt] = slot;
		}

		base._Ready();
	}
	
	public override bool AddCard(Card card, Slot slot)
	{
		GD.Print("Adding card from current deck");
		if (base.AddCard(card, slot))
		{
			EmitSignal(SignalName.AddCardMessage, card.CardID);
			return true;

		}
		return false;
	}
	public override bool AddCard(Card card)
	{
		GD.Print("Adding card from current deck");
		if (base.AddCard(card))
		{
			EmitSignal(SignalName.AddCardMessage, card.CardID);
			return true;
		}
		return false;
	}
	
	// This will allow all cards to be placed into the battlefield
	public void ActivateCardsInHand() {
		GD.Print("Activating All cards in hand");
		for (int i = 0; i < _cardLimit; i++)
		{
			if (_cardList[i] == null) { continue; }
			var card = _cardList[i];
			card.CurrentSlotStatus = Card.SlotStatus.Hand;
		}
	}
	
	// This will prevent cards to be placed into the battlefield
	public void PauseCardsInHand() {
		for (int i = 0; i < _cardLimit; i++)
		{
			if (_cardList[i] == null) { continue; }
			var card = _cardList[i];
			card.CurrentSlotStatus = Card.SlotStatus.HandPause;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
