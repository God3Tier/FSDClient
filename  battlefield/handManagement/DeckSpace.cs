namespace FSDClient.battlefield.handManagement;

using Godot;
using System;
using FSDClient.card.display;


public partial class DeckSpace : HandControl
{

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this._cardList = new Card[8];
		this._slotList = new DeckSlot[8];
		this._cardLimit = 8;
		foreach (Node child in GetChildren()) {
			string childName = child.Name.ToString();
			DeckSlot slot = (DeckSlot) child;
			int lastInt = (int)(childName[^1] - '1');
			_slotList[lastInt] = slot;
		}
		
		foreach (Slot slot in _slotList) {
			GD.Print(slot);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
