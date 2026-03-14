namespace FSDClient.battlefield;

using Godot;
using System;

public partial class HandArea : Control
{
	private float _raiseDistance = 470f;
	private float _raiseDuration = 0.5f;
	
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
		_raiseDuration)
		.SetTrans(Tween.TransitionType.Cubic)
		.SetEase(Tween.EaseType.Out);
	}
	
	public void LowerDeck()
	{
		if (!_isRaised) return;
		
		_isRaised = false;
		var tween = CreateTween();
		tween.TweenProperty(this, "position",
		_startPos,
		_raiseDuration)
		.SetTrans(Tween.TransitionType.Cubic)
		.SetEase(Tween.EaseType.Out);
	}
}
