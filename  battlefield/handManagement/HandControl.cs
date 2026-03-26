namespace FSDClient.battlefield.handManagement;

using Godot;
using System;
using System.Linq; // For the Contains method
using FSDClient.card.display;

public partial class HandControl : Control
{
	public Card[] _cardList { get; protected set; }
	public Slot[] _slotList { get; protected set; }
	protected int _cardCount = 0;
	protected int _cardLimit;
	protected float _normalSpeed = 0.3f;
	protected float _deckSpeed = 0.3f;
	
	// Add card to this hand control provided there is enough space
	public void AddCard(Card card)
	{
		// Check if the cardlist doesn't already has this specific card, and we still have space
		if (!_cardList.Contains(card) && _cardCount < _cardLimit) {
			for (int i = 0; i < _cardLimit; i ++) {
				// Find the first available slot
				if (!_slotList[i].CardInSlot) {
					_cardList[i] = card;
					_slotList[i].AddCard(card);
					_cardCount++;
					break;
				}
			}
			// update all the card positions
			UpdateCardPositions();
		}
		// If the card is already part of this collection, animate it back to its original slot
		else
		{
			int index = Array.IndexOf(_cardList, card);
			AnimateCardToPosition(card, _slotList[index].GlobalPosition, _normalSpeed);
		}
	}
	
	// Add card to a specific slot
	public void AddCard(Card card, Slot slot)
	{
		// If the slot is not part of this group, don't do anything
		if (!_slotList.Contains(slot))
		{
			GD.Print($"{this.Name} doesn't have {slot.Name}");
			return;
		}
		
		// If the slot has a card already, don't do anything
		if (slot.CardInSlot)
		{
			return;
		}
		
		// If the card doesn't already exist in the space, add it
		if (!_cardList.Contains(card))
		{
			int index = Array.IndexOf(_slotList, slot);
			_slotList[index].AddCard(card);
			_cardList[index] = card;
			_cardCount++;
			UpdateCardPositions();
		}
		else
		{
			int index = Array.IndexOf(_cardList, card);
			AnimateCardToPosition(card, _slotList[index].GlobalPosition, _normalSpeed);
		}
	}
	
	// To remove the card from the slot
	public void RemoveCard(Card card) 
	{
		int index = Array.IndexOf(_cardList, card);
		_cardList[index] = null;
		_slotList[index].RemoveCard();
		_cardCount--;
		UpdateCardPositions();
	}
	
	
	// To animate 1 card back to its position
	public void AnimateCardToPosition(Card card, Vector2 position, float speed) 
	{
		GD.Print($"{card.Name} called with {position}");
		if (card.MoveTween != null && card.MoveTween.IsRunning())
		{
			card.MoveTween.Kill();
		}
        
        
		var tween = GetTree().CreateTween();
		card.MoveTween = tween;
		tween.TweenProperty(card, "global_position", position, speed);
	}
	
	// To animate all cards back to its position
	public void UpdateCardPositions()
	{
		for (int i = 0; i < _cardLimit; i++)
		{
			if (_cardList[i] == null) { continue; }
			var newPosition = _slotList[i].GlobalPosition;
			var card = _cardList[i];
			card.StartingPosition = newPosition;
			AnimateCardToPosition(card, newPosition, _normalSpeed);
		}
	}
	
	// To move all cards with the whole area
	public void AnimateAllCardsToPosition(float distance, bool isRaise) 
	{
		GD.Print($"Animating cards for isRaise {isRaise}");
		for (int i = 0; i < _cardLimit; i++)
		{
			if (_cardList[i] == null) { continue; }
			var card = _cardList[i];
			if (isRaise) {
				var newPosition = card.StartingPosition - new Vector2(0, distance);
				AnimateCardToPosition(card, newPosition, _deckSpeed);
			}
			else {
				var newPosition = card.StartingPosition + new Vector2(0, distance);
				AnimateCardToPosition(card, newPosition, _deckSpeed);
			}
		}
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
