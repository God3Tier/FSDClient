namespace FSDClient.card.display;

using Godot;
using System;
using FSDClient.battlefield.handManagement;

public partial class Card : Sprite2D
{
    [Signal] public delegate void HoveredEventHandler(Card card);
    [Signal] public delegate void HoveredOffEventHandler(Card card);
    public Vector2 StartingPosition;
    // hi 

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // GD.Print("Ready successfully called");
        try
        {
            var CardManager = GetParent<CardManager>();
            if (CardManager != null)
            {
                Hovered += CardManager.OnHoverOverCard;
                HoveredOff += CardManager.OnHoverOffCard;
            }
            GD.Print("Successfully initialised Parent Card Manager");

        }
        catch (Exception e)
        {
            GD.PrintErr("Whoops ", e);
        }

        
        var area = (Area2D)FindChild("Area2D", true);
        area.SetCollisionLayerValue(1, true);
        area.MouseEntered += OnMouseEntered;
        area.MouseExited += OnMouseExited;
        
    
        // DesignatedArea = new();
        // DesignatedArea.Name = "Area2D";
        // DesignatedArea.SetCollisionLayerValue(1, true);

        // var shape = new CollisionShape2D();
        // var rectShape = new RectangleShape2D();
        // // To be settled later when I fixed all the resizing 
        // rectShape.Size = new Vector2(1000, 1000);
        // shape.Shape = rectShape;
        // DesignatedArea.AddChild(shape);

        var control = FindChild("CardView", true);
        var texture = (TextureRect)control.FindChild("Icon", true);
        texture.Texture = GD.Load<Texture2D>("res://assets/symbols/attack_symbol.png");
        
    }

    // public void InitializeCard(CardView CardView)
    // {

    //     var control = FindChild("CardView", true); 
    //     AddChild(CardView);
    //     GD.Print("Initialized the card into the Card");
    // }

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
