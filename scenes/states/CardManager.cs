using Godot;
using System;

public partial class CardManager : Node2D
{
	private Node2D cardBeingDragged;
	private Vector2 dragOffset;
	private Vector2 screenSize;
	
	// For handling game input
	public override void _Input(InputEvent inputEvent)
	{
		// Check for mouse button
		if (inputEvent is InputEventMouseButton mouseEvent)
		{
			// Check if it was left button
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (mouseEvent.IsPressed()) {
					var card = _raycastCheckForCard();
					if (card != null)
					{
						cardBeingDragged = card;
						dragOffset = cardBeingDragged.GlobalPosition - GetGlobalMousePosition();

					}
				}
				else {
					cardBeingDragged = null;
				}
			}
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
			CollisionMask = 1
		};
		
		var result = spaceState.IntersectPoint(parameters);
		if (result.Count > 0) {
			var collider = (Area2D)result[0]["collider"];
			var cardParent = collider.GetParent<Node2D>();
			dragOffset = cardParent.GlobalPosition - GetGlobalMousePosition();
			return cardParent;
		}
		return null;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// TODO: Find out why there is a bug with this = screenSize is xxx,2 when using this
		// screenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (cardBeingDragged != null) {
			screenSize = GetWindow().Size;
			var mousePosition = GetGlobalMousePosition() + dragOffset;
			var clampedPosition = mousePosition.Clamp(Vector2.Zero, screenSize);
			cardBeingDragged.GlobalPosition = clampedPosition;
			
		}
	}
}
