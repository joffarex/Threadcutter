using Godot;
using Threadcutter.DataObjects;

namespace Threadcutter.Components;

public partial class VelocityComponent : Node
{
    public Vector2 Velocity { get; set; }

    [Export] public bool IsGravityDisabled { get; set; }
    [Export] public CharacterData CharacterData { get; set; }

    [Export] public float Speed { get; set; }
    
    [Export] public JumpData JumpData { get; set; }
    [Export] public CharacterBody2D CharacterBody { get; set; }


    public override void _PhysicsProcess(double delta)
    {
        HandleGravity(delta);

        CharacterBody.Velocity = Velocity;
        CharacterBody.MoveAndSlide();
    }

    private void HandleGravity(double delta)
    {
        if (IsGravityDisabled)
        {
            return;
        }

        float gravity = Velocity.Y < 0.0 ? JumpData.JumpGravity : JumpData.FallGravity;

        if (!CharacterBody.IsOnFloor())
        {
            Velocity = Velocity with { Y = Velocity.Y + gravity * (float)delta };
        }
    }

    public void MoveInDirection(float direction)
    {
        if (direction == 0.0 || CharacterData.IsSuppressed)
        {
            Velocity = Velocity with { X = 0.0f };
        }
        else
        {
            Velocity = Velocity with { X = direction * Speed };
        }
    }

    public void MoveTowards(Vector2 targetPosition)
    {
        Velocity = (targetPosition - CharacterBody.Position).Normalized() * Speed;
    }
}