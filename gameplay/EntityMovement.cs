using Godot;

namespace FSDClient.gameplay;

public partial class EntityMovement : Area2D
{
    private int Speed { get; } = 40000;
    private Vector2 CurrPosition { get; set; }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shdx)
    {
        if (@event is InputEventMouseButton inputMouseButton && inputMouseButton.Position == CurrPosition)
        {

        }
    }

    public override void _PhysicsProcess(double delta)
    {

    }
}
