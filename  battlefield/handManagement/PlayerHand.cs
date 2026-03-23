using Godot;
using System;
using System.Linq;
using FSDClient.card.display;


namespace FSDClient.battlefield.handManagement;


public partial class PlayerHand : Control
{
	const string CARD_SCENE_PATH = "res://scenes/gameComponents/Card.tscn";
	private Card[] Hand = new Card[4];
	private HandSlot[] HandSlots = new HandSlot[4];
	private int HAND_COUNT = 0;
	private int HAND_Y_POSITION = 4;
	private int CARD_WIDTH = 100;
	private int centerScreenX;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node child in GetChildren()) {
			string childName = child.Name.ToString();
			HandSlot slot = (HandSlot) child;
			int lastInt = (int)(childName[^1] - '1');
			HandSlots[lastInt] = slot;
		}

		foreach (HandSlot slot in HandSlots) {
			GD.Print(slot);
		}
		// centerScreenX = GetWindow().Size.X / 2;
		// var cardScene = GD.Load<PackedScene>(CARD_SCENE_PATH);
		// var cardManager = GetNode<Node2D>("../CardManager");
		// for (int i = 0; i < 4; i++)
		// {
		//     var newCard = cardScene.Instantiate<Card>();
		//     if (newCard == null) { GD.PrintErr("Failed to instantiate Card!"); continue; }
		//     cardManager.AddChild(newCard);
		//     newCard.Name = "Card";
		//     AddCardToHand(newCard);
		// }
	}

	public void AddCardToHand(Card card)
	{
		if (!Hand.Contains(card) && HAND_COUNT < 4) {
			for (int i = 0; i < 4; i ++) {
				if (Hand[i] == null) {
					Hand[i] = card;
					break;
				}

			}
			UpdateHandPositions();
		}
		else
		{
			AnimateCardToPosition(card, card.StartingPosition);
		}

	}

	public void UpdateHandPositions()
	{
		for (int i = 0; i < Hand.Length; i++)
		{
			if (Hand[i] == null) { continue; }
			var newPosition = HandSlots[i].GlobalPosition;
			var card = Hand[i];
			card.StartingPosition = newPosition;
			AnimateCardToPosition(card, newPosition);
		}
	}


	public void AnimateCardToPosition(Card card, Vector2 position)
	{
		var tween = GetTree().CreateTween();
		tween.TweenProperty(card, "global_position", position, 0.3);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
