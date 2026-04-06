namespace FSDClient.battlefield.handManagement;

using Godot;
using System;
using System.Linq; // For the Contains method
using FSDClient.card.display;

public partial class HandControl : Control
{
	public Card[] _cardList { get; protected set; }
	public Slot[] _slotList { get; protected set; }
	protected Vector2[] _slotBasePositions;
	protected int _cardCount = 0;
	protected int _cardLimit;
	protected float _normalSpeed = 0.3f;
	protected float _deckSpeed = 0.3f;
	protected float _offset = 470f;
	protected bool _isRaised = true;
	
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
			/// Signal Emission 
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
		
        // Emit Signal 
	}
	
	
	// To animate 1 card back to its position
	public void AnimateCardToPosition(Card card, Vector2 position, float speed) 
	{
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
		GD.Print($"Updating the cards in {this.Name}");
		for (int i = 0; i < _cardLimit; i++)
		{
			if (_cardList[i] == null) { continue; }
			var basePosition = _slotBasePositions[i];
			var newPosition = this._isRaised ? basePosition - new Vector2(0, _offset) : basePosition;
			var card = _cardList[i];
			card.StartingPosition = newPosition;
			if (_isRaised)
			{
				GD.Print($"{card.Name} is raised, moving to {card.StartingPosition}");
			}
			else
			{
				GD.Print($"{card.Name} is not raised, moving to {card.StartingPosition}");
			}
			AnimateCardToPosition(card, newPosition, _normalSpeed);
		}
	}
	
	// To move all cards with the whole area
	public void AnimateAllCardsToPosition(bool isRaised) 
	{
		this._isRaised = isRaised;
		UpdateCardPositions();
	}
	
	// For debugging the _slotBasePositions
	public void _debugSlot()
	{
		for (int i = 0; i < _cardLimit; i++)
		{
		GD.Print($"{_slotList[i].Name} - {_slotBasePositions[i]}");
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_slotBasePositions = new Vector2[_cardLimit];
		for (int i = 0; i < _cardLimit; i++)
			{
				_slotBasePositions[i] = _slotList[i].GlobalPosition;
			}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
