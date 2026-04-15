namespace FSDClient.battlefield.handManagement;

using Godot;
using System;
using System.Linq; // For the Contains method
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
	public DeckSpace _deckSpace { get; set; }
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
					else
					{
						return;
					}
				}
				else
				{
					StopDrag();
				}
			}
		}
	}
	
	// When the timer is out, call stop drag if we are still dragging a card
	public void UnstuckCard()
	{
		if (cardBeingDragged != null)
		{
			StopDrag();
		}
	}
	
	// debug printer
	public void CheckHandAndDeck()
	{
		foreach (Slot slot in _playerHand._slotList) {
			if (slot != null && slot.CardInSlot)
			{
				GD.Print($"{slot.Name}: {slot.Card.Name}");
			}
			else if (slot != null)
			{
			GD.Print($"{slot.Name}: none");
			}
		}
		
		foreach (Slot slot in _deckSpace._slotList) {
			if (slot != null && slot.CardInSlot)
			{
				GD.Print($"{slot.Name}: {slot.Card.Name}");
			}
			else if (slot != null)
			{
			GD.Print($"{slot.Name}: none");
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
		GD.Print(cardBeingDragged.CurrentSlotStatus);
		// Put the signal here for nonsence in gameloop
		// TODO: DO it
		// First check if the slot is a BattleSlot,
		// then check if there is a card in the slot already,
		// then check if the card is currently allowed to be added
		if (slotFound is BattleSlot battleSlotFound &&
			!battleSlotFound.CardInSlot &&
			cardBeingDragged.CurrentSlotStatus == Card.SlotStatus.Hand &&
			(bool)battleSlotFound.GetMeta("MySlot"))
		{
			IntoBattleSlot(battleSlotFound);
		}
		else if (slotFound is HandSlot handSlotFound && 
				!handSlotFound.CardInSlot && 
				cardBeingDragged.CurrentSlotStatus == Card.SlotStatus.Deck)
		{
			IntoHandSlot(handSlotFound);
		}
		else if (slotFound is DeckSlot deckSlotFound &&
				cardBeingDragged.CurrentSlotStatus == Card.SlotStatus.HandTemp)
		{
			ReturnToDeckSlot(deckSlotFound);
		}
		else if (_playerHand._cardList.Contains(cardBeingDragged))
		{
			_playerHand.AddCard((Card)cardBeingDragged);
			GD.Print("Return to hand");
		}
		else
		{
			_deckSpace.AddCard((Card)cardBeingDragged);
			GD.Print("Return to deck");
		}
		cardBeingDragged = null;

	}

	// To handle the logic of a card entering a battle slot
	private void IntoBattleSlot(BattleSlot battleSlotFound) {
		// Set the card's position to the batte slot
		cardBeingDragged.Position = battleSlotFound.Position;
		cardBeingDragged.ZIndex = 2;
		// To ensure another card cannot go into the battle slot
		battleSlotFound.AddCard(cardBeingDragged);
		// To deactivate interactions with the card that entered the battle slot
		var collisionShape = cardBeingDragged.GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
		collisionShape.Disabled = true;
		cardBeingDragged.CurrentSlotStatus = Card.SlotStatus.Battle;
		// To remove the card from the players hand and free up a slot
		_playerHand.RemoveCard((Card)cardBeingDragged);
		EmitSignal(SignalName.CardDropped, battleSlotFound);
	}
	
	private void BounceBattleSlot(BattleSlot slot) {
		Card bouncedCard = slot.Card;
		slot.RemoveCard();
		// TODO: Might want to rebuild a card instead and add it into hand if possible
		_playerHand.AddCard(bouncedCard);
	}
	
	// For putting a card into the hand
	private void IntoHandSlot(HandSlot handSlotFound) 
	{
		cardBeingDragged.CurrentSlotStatus = Card.SlotStatus.HandTemp;
		_deckSpace.RemoveCard(cardBeingDragged);
		_playerHand.AddCard(cardBeingDragged, handSlotFound);
	}
	
	// For returning a card back into the deck between rounds
	private void ReturnToDeckSlot(DeckSlot deckSlotFound)
	{
		cardBeingDragged.CurrentSlotStatus = Card.SlotStatus.Deck;
		_playerHand.RemoveCard(cardBeingDragged);
		_deckSpace.AddCard(cardBeingDragged, deckSlotFound);
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
		if (card.CurrentSlotStatus == Card.SlotStatus.Battle) {return;}
		if (!highlighting)
		{
			highlighting = true;
			HighlightCard(card, true);
		}
	}

	// Hover off Card effect
	public void OnHoverOffCard(Card card)
	{
		if (card.CurrentSlotStatus == Card.SlotStatus.Battle) {return;}
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
			card.ZIndex = 5;
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
			var slot = GetHighestZIndex(result);
			if (slot is Slot) {
				return (Slot) slot;
			}

		}
		return null;
	}

	// To retrieve the node that is rendered at the top
	private Node2D GetHighestZIndex(Godot.Collections.Array<Godot.Collections.Dictionary> result)
	{
		var HighestZCollider = (Area2D)result[0]["collider"];
		var HighestZNode = HighestZCollider.GetParent<Node2D>();
		for (int i = 0; i < result.Count; i++)
		{
			var collider = (Area2D)result[i]["collider"];
			var node = collider.GetParent<Node2D>();
			if (node.ZIndex > HighestZNode.ZIndex)
			{
				HighestZNode = node;
			}
		}
		return HighestZNode;
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
