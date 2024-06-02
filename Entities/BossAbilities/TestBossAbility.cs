using Godot;
using Threadcutter.Autoload;

namespace Threadcutter.Entities.BossAbilities;

public partial class TestBossAbility : Node2D
{
    [Export] public Vector2 InstanciatePosition { get; set; } = Vector2.Zero;
    
    
    public override void _Ready()
    {
        GetTree().CreateTimer(1.0f, false).Timeout += QueueFree;
        InstanciatePosition = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (InstanciatePosition != Vector2.Zero)
        {
            var mousePosition = GetGlobalMousePosition();

            var targetDirection = (mousePosition - InstanciatePosition).Normalized();
            var currentDirection = Position.DirectionTo(mousePosition);

            var interpolatedDirection = MathUtils.Lerp(currentDirection, targetDirection, 0.01f);

            Position += interpolatedDirection * 450.0f * (float)delta;
        }
    }
}