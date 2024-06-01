using Godot;
using Threadcutter.DataObjects;
using Threadcutter.Entities.Characters;

namespace Threadcutter.Components;

public partial class CharacterAnimator : Node
{
    
    [Export] public CharacterBody2D CharacterBody { get; set; }
    [Export] public CharacterData CharacterData { get; set; }
    [Export] public MovementFiniteStateMachine MovementFiniteStateMachine { get; set; }
    [Export] public CombatFiniteStateMachine CombatFiniteStateMachine { get; set; }

    [Export] public AnimationPlayer AnimationPlayer { get; set; }

    
    public override void _Process(double delta)
    {
        if (CombatFiniteStateMachine.CurrentState != "no_attack")
        {
            AnimationPlayer.Play(CombatFiniteStateMachine.CurrentState);
        }
        else
        {
            switch (MovementFiniteStateMachine.CurrentState)
            {
                case "idle":
                    if (CharacterBody.IsOnFloor())
                    {
                        AnimationPlayer.Play(AnimationPlayer.CurrentAnimation == "land" ? "land" : "idle");
                    }

                    break;
                case "move":
                    if (CharacterBody.IsOnFloor())
                    {
                        AnimationPlayer.Play("move");
                    }

                    break;
                case "jump":
                    AnimationPlayer.Play("jump");
                    break;
                case "coyote_jump":
                    AnimationPlayer.Play("fall");

                    if (CharacterBody.IsOnFloor())
                    {
                        AnimationPlayer.Play("land");
                    }
                
                    break;
                case "fall":
                    AnimationPlayer.Play("fall");
                
                    if (CharacterBody.IsOnFloor())
                    {
                        AnimationPlayer.Play("land");
                    }

                    break;
                default:
                    GD.Print($"[WARN] Incorrect ANIMATION state {MovementFiniteStateMachine.CurrentState}");
                    break;
            }
        }
        
        
    }
}