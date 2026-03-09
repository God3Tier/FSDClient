namespace FSDClient.battlefield.handManagement;

using Godot;
using System;
using FSDClient.card.display;


public partial class CardManager : Node2D
{
	private Node2D cardBeingDragged;
	private Vector2 dragOffset;
	private Vector2 screenSize;
	private Boolean highlighting = false;
	private uint COLLISION_MASK_CARD = 1;
	private uint COLLISION_MASK_CARD_SLOT = 2;
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
					var card = _raycastCheckForCard();
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

	private void StartDrag(Node2D card)
	{
		cardBeingDragged = card;
		dragOffset = cardBeingDragged.GlobalPosition - GetGlobalMousePosition();
		card.Scale = new Vector2(1f, 1f);
	}

	private void StopDrag()
	{
		cardBeingDragged.Scale = new Vector2(1.05f, 1.05f);
		var cardSlotFound = _raycastCheckForCardSlot();
		if (cardSlotFound != null && !cardSlotFound.CardInSlot)
		{
			cardBeingDragged.Position = cardSlotFound.Position;
			cardSlotFound.CardInSlot = true;
			var collisionShape = cardBeingDragged.GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
			collisionShape.Disabled = true;
		}
		else
		{
			handReference.AddCardToHand((Card)cardBeingDragged);
		}
		cardBeingDragged = null;

	}

	// For the hover effect
	private void ConnectCardSignal(Card card)
	{
		card.Hovered += OnHoverOverCard;
		card.HoveredOff += OnHoverOffCard;
	}

	public void OnHoverOverCard(Card card)
	{
		if (!highlighting)
		{
			highlighting = true;
			HighlightCard(card, true);
		}
	}

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

	private void HighlightCard(Card card, bool hovered)
	{
		if (hovered)
		{
			card.Scale = new Vector2(1.05f, 1.05f);
			card.ZIndex = 2;
		}
		else
		{
			card.Scale = new Vector2(0.95f, 0.95f);
			card.ZIndex = 1;
		}
	}

	// To return what is under our cursor when clicking
	public Node2D _raycastCheckForCard()
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
			dragOffset = cardParent.GlobalPosition - GetGlobalMousePosition();
			return cardParent;
		}
		return null;
	}

	// To return what is under our cursor when clicking
	public CardSlot? _raycastCheckForCardSlot()
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
			var cardParent = collider.GetParent<CardSlot>();
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
