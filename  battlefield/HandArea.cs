namespace FSDClient.battlefield;

using Godot;
using System;
using FSDClient.battlefield.handManagement;

public partial class HandArea : Control
{
	public PlayerHand _playerHand { get; set; }
	public DeckSpace _deckSpace {get; set;}
	private float _raiseDistance = 470f;
	private float _raiseDuration = 0.3f;

	private Vector2 _startPos;
	private bool _isRaised = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_startPos = Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void RaiseDeck()
	{
		if (_isRaised) return;

		_isRaised = true;
		var tween = CreateTween();
		tween.TweenProperty(this, "position",
		_startPos - new Vector2(0, _raiseDistance),
		_raiseDuration);
		
		if (_playerHand != null) {
			_playerHand.AnimateAllCardsToPosition(_raiseDistance/128, true);
		}
		if (_deckSpace != null) {
			_deckSpace.AnimateAllCardsToPosition(_raiseDistance, true);
		}
		
	}

	public void LowerDeck()
	{
		if (!_isRaised) return;

		_isRaised = false;
		var tween = CreateTween();
		tween.TweenProperty(this, "position",
		_startPos,
		_raiseDuration);
		if (_playerHand != null) {
			_playerHand.AnimateAllCardsToPosition(_raiseDistance, false);
		}
		if (_deckSpace != null) {
			_deckSpace.AnimateAllCardsToPosition(_raiseDistance/64, false);
		}
	}
}
