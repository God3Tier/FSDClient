namespace FSDClient.battlefield.handManagement;

using Godot;
using System;
using System.Linq;
using FSDClient.card.display;


public partial class PlayerHand : HandControl
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this._cardList = new Card[4];
		this._slotList = new HandSlot[4];
		this._cardLimit = 4;
		foreach (Node child in GetChildren()) {
			string childName = child.Name.ToString();
			HandSlot slot = (HandSlot) child;
			int lastInt = (int)(childName[^1] - '1');
			_slotList[lastInt] = slot;
		}
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
