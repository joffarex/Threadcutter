using System.Diagnostics;
using Godot;
using Threadcutter.Components;
using Threadcutter.DataObjects;

namespace Threadcutter.Entities.Characters;

public partial class Character : CharacterBody2D
{
    public CharacterMovementFiniteStateMachine CharacterMovementFiniteStateMachine { get; set; }
    public CharacterCombatFiniteStateMachine CharacterCombatFiniteStateMachine { get; set; }

    public AnimationPlayer AnimationPlayer { get; set; }
    public BossAbilityManager BossAbilityManager { get; set; }
    public XpManager XpManager { get; set; }
    public AbilityEvolver AbilityEvolver { get; set; }
    
    
    [Export] public CharacterData CharacterData { get; set; }
    
    public override void _Ready()
    {
        CharacterMovementFiniteStateMachine = GetNode<Components.CharacterMovementFiniteStateMachine>("CharacterMovementFiniteStateMachine");
        CharacterCombatFiniteStateMachine = GetNode<Components.CharacterCombatFiniteStateMachine>("CharacterCombatFiniteStateMachine");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        BossAbilityManager = GetNode<BossAbilityManager>("BossAbilityManager");
        XpManager = GetNode<XpManager>("XpManager");
        AbilityEvolver = GetNode<AbilityEvolver>("AbilityEvolver");

        XpManager.GainedLevel += level =>
        {
            if (level % CharacterData.EvolvePerLevelAmount == 0)
            {
                var evolves = AbilityEvolver.SuggestAbilityEvolves();
                GD.Print($"{evolves.Item1} | {evolves.Item2}");
            }
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        float direction = Input.GetAxis("left", "right");

        ResolveDirection(direction);
        
        CharacterMovementFiniteStateMachine.ProcessCurrentState(direction, delta);
        CharacterCombatFiniteStateMachine.ProcessCurrentState(direction, delta);

        Debug();
    }

    private void ResolveDirection(float inputDirection)
    {
        if (inputDirection != 0.0f)
        {
            Transform = Transform with { X = Transform.X with { X = inputDirection } };
            CharacterData.CurrentDirection = inputDirection;
        }
    }

    private void Debug()
    {
        if (Input.IsActionJustPressed("debug_btn"))
        {
            if (XpManager.CurrentLevel % CharacterData.EvolvePerLevelAmount == 0)
            {
                var evolves = AbilityEvolver.SuggestAbilityEvolves();
                GD.Print($"{evolves.Item1} | ${evolves.Item2}");
            }
        }
    }
}