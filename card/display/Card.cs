namespace FSDClient.card.display;

using Godot;
using System;
using FSDClient.battlefield.handManagement;

public partial class Card : Sprite2D
{
    [Signal] public delegate void HoveredEventHandler(Card card);
    [Signal] public delegate void HoveredOffEventHandler(Card card);
    private CardView CurrentCard;
    public Vector2 StartingPosition;
    private Area2D DesignatedArea;
    private CardManager cardManager;
    // hi 

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // GD.Print("Ready successfully called");
        // foreach (Node child in GetChildren())
        // {
        //     GD.Print("Has Child ", child.GetType().FullName);
        //     if (child is Area2D area)
        //     {
        //         area.SetCollisionLayerValue(1, true);
        //         area.MouseEntered += OnMouseEntered;
        //         area.MouseExited += OnMouseExited;
        //     }
        // }
        DesignatedArea = new();
        DesignatedArea.Name = "Area2D";
        DesignatedArea.SetCollisionLayerValue(1, true);

        var shape = new CollisionShape2D();
        var rectShape = new RectangleShape2D();
        // To be settled later when I fixed all the resizing 
        rectShape.Size = new Vector2(1000, 1000);
        shape.Shape = rectShape;
        DesignatedArea.AddChild(shape);

        // Connect signals for hover
        DesignatedArea.MouseEntered += OnMouseEntered;
        DesignatedArea.MouseExited += OnMouseExited;

        AddChild(DesignatedArea);
        CallDeferred(nameof(DefferedInitialiser));
    }

    private void DefferedInitialiser()
    {
        try
        {
            var CardManager = GetParent<CardManager>();
            if (CardManager != null)
            {
                Hovered += CardManager.OnHoverOverCard;
                HoveredOff += CardManager.OnHoverOffCard;
            }
            GD.Print("Successgully initialised Parent Card Manager");

        }
        catch (Exception e)
        {
            GD.PrintErr("Whoops ", e);
        }
    }
    public void InitializeCard(CardView CardView)
    {

        CurrentCard = CardView;
        AddChild(CardView);
        GD.Print("Initialized the card into the Card");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void OnMouseEntered()
    {
        EmitSignal(SignalName.Hovered, this);
    }

    public void OnMouseExited()
    {
        EmitSignal(SignalName.HoveredOff, this);
    }

}
