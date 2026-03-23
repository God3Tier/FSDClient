namespace FSDClient.battlefield.handManagement;

using Godot;
using System;
using FSDClient.card.display;


public partial class CardManager : Node2D
{

	[Signal]
	public delegate void CardDroppedEventHandler(BattleSlot battleSlot);

	private Card cardBeingDragged;
	private Vector2 dragOffset;
	private Vector2 screenSize;
	private Boolean highlighting = false;
	private uint COLLISION_MASK_CARD = 1;
	private uint COLLISION_MASK_CARD_SLOT = 2;
	public PlayerHand _playerHand  { get; set; }
	public Control _deck { get; set; }
	private PlayerHand handReference;

	// For handling game input
	public override void _Input(InputEvent inputEvent)
	{
		// Check for mouse button
		if (inputEvent is InputEventMouseButton mouseEvent)
		{
			// Check if it was left button
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{

				if (mouseEvent.IsPressed())
				{
					Card card = _raycastCheckForCard();
					if (card != null)
					{
						StartDrag(card);
					}
				}
				else
				{
					StopDrag();
				}
			}
		}
	}

	// To start drag
	private void StartDrag(Card card)
	{
		cardBeingDragged = card;
		dragOffset = cardBeingDragged.GlobalPosition - GetGlobalMousePosition();
		card.Scale = new Vector2(1f, 1f);
	}

	// To stop drag
	private void StopDrag()
	{
		cardBeingDragged.Scale = new Vector2(1.005f, 1.005f);
		var slotFound = _raycastCheckForSlot();

		// Put the signal here for nonsence in gameloop
		// TODO: DO it
		if (slotFound is BattleSlot battleSlotFound && !battleSlotFound.CardInSlot)
		{
			cardBeingDragged.Position = battleSlotFound.Position;
			cardBeingDragged.ZIndex = 1;
			GD.Print(battleSlotFound.Position);
			GD.Print(cardBeingDragged.Position);

			GD.Print(dragOffset);
			battleSlotFound.CardInSlot = true;
			var collisionShape = cardBeingDragged.GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
			collisionShape.Disabled = true;
			battleSlotFound.Card = (Card)cardBeingDragged;

			EmitSignal(SignalName.CardDropped, battleSlotFound);
		}
		else
		{
			_playerHand.AddCardToHand((Card)cardBeingDragged);
		}
		cardBeingDragged = null;

	}

	// For the hover effect
	private void ConnectCardSignal(Card card)
	{
		card.Hovered += OnHoverOverCard;
		card.HoveredOff += OnHoverOffCard;
	}

	// Hover Card effect
	public void OnHoverOverCard(Card card)
	{
		if (!highlighting)
		{
			highlighting = true;
			HighlightCard(card, true);
		}
	}

	// Hover off Card effect
	public void OnHoverOffCard(Card card)
	{
		if (cardBeingDragged == null)
		{
			HighlightCard(card, false);
			var NewCard = (Card)_raycastCheckForCard();
			if (NewCard != null)
			{
				HighlightCard(NewCard, true);
			}
			else
			{
				highlighting = false;
			}
		}
	}

	// To make the card fly up to the top of the ZIndex, or back down
	private void HighlightCard(Card card, bool hovered)
	{
		if (hovered)
		{
			card.Scale = new Vector2(1.05f, 1.05f);
			card.ZIndex = 5;
		}
		else
		{
			card.Scale = new Vector2(0.95f, 0.95f);
			card.ZIndex = 4; // TODO: This will likely cause problems with the hand later
		}
	}

	// To return what is under our cursor when clicking
	public Card _raycastCheckForCard()
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		var parameters = new PhysicsPointQueryParameters2D
		{
			Position = GetGlobalMousePosition(),
			CollideWithAreas = true,
			CollisionMask = COLLISION_MASK_CARD
		};

		var result = spaceState.IntersectPoint(parameters);
		if (result.Count > 0)
		{
			var cardParent = GetHighestZIndex(result);
			if (cardParent is Card) {
				dragOffset = cardParent.GlobalPosition - GetGlobalMousePosition();
				return (Card) cardParent;
			}

		}
		return null;
	}

	// To return what is under our cursor when clicking
	public Slot? _raycastCheckForSlot()
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		var parameters = new PhysicsPointQueryParameters2D
		{
			Position = GetGlobalMousePosition(),
			CollideWithAreas = true,
			CollisionMask = COLLISION_MASK_CARD_SLOT
		};

		var result = spaceState.IntersectPoint(parameters);
		if (result.Count > 0)
		{
			var collider = (Area2D)result[0]["collider"];
			var cardParent = collider.GetParent<Slot>();
			return cardParent;
		}
		return null;
	}

	private Node2D GetHighestZIndex(Godot.Collections.Array<Godot.Collections.Dictionary> result)
	{
		var HighestZCollider = (Area2D)result[0]["collider"];
		var HighestZCard = HighestZCollider.GetParent<Node2D>();
		for (int i = 0; i < result.Count; i++)
		{
			var collider = (Area2D)result[i]["collider"];
			var card = collider.GetParent<Node2D>();
			if (card.ZIndex > HighestZCard.ZIndex)
			{
				HighestZCard = card;
			}
		}
		return HighestZCard;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// TODO: Find out why there is a bug with this = screenSize is xxx,2 when using this
		// screenSize = GetViewportRect().Size;

		// Connect all the cards
		// TODO: Fix the signal connection so that one card doesn't trigger all cards
		foreach (Node child in GetChildren())
		{
			if (child is Card card)
			{
				ConnectCardSignal(card);
			}
		}

		handReference = GetNode<PlayerHand>("../PlayerHand");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (cardBeingDragged != null)
		{
			screenSize = GetWindow().Size;
			var mousePosition = GetGlobalMousePosition() + dragOffset;
			var clampedPosition = mousePosition.Clamp(Vector2.Zero, screenSize);
			cardBeingDragged.GlobalPosition = clampedPosition;

		}
	}
}
