using Godot;
using System;
using System.Linq;
using FSDClient.card.display;

namespace FSDClient.battlefield.handManagement;


public partial class PlayerHand : Node2D
{
    const string CARD_SCENE_PATH = "res://scenes/gameComponents/Card.tscn";
    private Card[] Hand = new Card[4];
    private int centerScreenX;
    private int HAND_COUNT = 0;
    private int CARD_WIDTH = 300;
    private int HAND_Y_POSITION = 850;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
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
        if (!Hand.Contains(card))
        {
            Hand[HAND_COUNT] = card;
            HAND_COUNT++;
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
            var newPosition = new Vector2(CalculateHandPosition(i), HAND_Y_POSITION);
            var card = Hand[i];
            card.StartingPosition = newPosition;
            AnimateCardToPosition(card, newPosition);
        }
    }

    public int CalculateHandPosition(int index)
    {
        var totalWidth = (Hand.Length - 1) * CARD_WIDTH;
        var xOffset = centerScreenX + index * CARD_WIDTH + totalWidth / 2;
        return xOffset;
    }

    public void AnimateCardToPosition(Card card, Vector2 position)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(card, "position", position, 0.3);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
