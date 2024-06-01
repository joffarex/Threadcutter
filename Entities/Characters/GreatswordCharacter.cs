using Godot;
using Threadcutter.DataObjects;

namespace Threadcutter.Entities.Characters;

public partial class GreatswordCharacter : CharacterBody2D
{
    public Components.MovementFiniteStateMachine MovementFiniteStateMachine { get; set; }

    public AnimationPlayer AnimationPlayer { get; set; }
    [Export] public CharacterData CharacterData { get; set; }
    
    public override void _Ready()
    {
        MovementFiniteStateMachine = GetNode<Components.MovementFiniteStateMachine>("MovementFiniteStateMachine");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
        float direction = Input.GetAxis("left", "right");

        ResolveDirection(direction);
        
        MovementFiniteStateMachine.ProcessCurrentState(direction, delta);
    }

    private void ResolveDirection(float inputDirection)
    {
        if (inputDirection != 0.0f)
        {
            Transform = Transform with { X = Transform.X with { X = inputDirection } };
            CharacterData.CurrentDirection = inputDirection;
        }
    }
}