using Godot;
using Threadcutter.DataObjects;
using Threadcutter.Scripts;

namespace Threadcutter.Components;

public partial class CombatFiniteStateMachine : FiniteStateMachine
{
    [Export] public CharacterBody2D CharacterBody { get; set; }
    [Export] public CharacterData CharacterData { get; set; }
    [Export] public MovementFiniteStateMachine MovementFiniteStateMachine { get; set; }

    private Timer _mainAttackDurationTimer;
    private Timer _secondaryAttackDurationTimer;

    private const float AttackBufferTime = 0.2f;

    public override void _Ready()
    {
        base._Ready();
        
        _mainAttackDurationTimer = new Timer();
        _mainAttackDurationTimer.WaitTime = 0.5f; // Taken from `AnimationPlayer`
        _mainAttackDurationTimer.OneShot = true;
        AddChild(_mainAttackDurationTimer);
        
        _secondaryAttackDurationTimer = new Timer();
        _secondaryAttackDurationTimer.WaitTime = 0.7f; // Taken from `AnimationPlayer`
        _secondaryAttackDurationTimer.OneShot = true;
        AddChild(_secondaryAttackDurationTimer);
    }

    private bool CanSecondaryAttack() =>
        MovementFiniteStateMachine.CurrentState != "jump" &&
        MovementFiniteStateMachine.CurrentState != "fall" &&
        MovementFiniteStateMachine.CurrentState != "coyote_jump";
    public override void EnterState(string state)
    {
        switch (state)
        {
            case "no_attack":
                return;
            case "main_attack":
                _mainAttackDurationTimer.Start();
                return;
            case "secondary_attack":
                _secondaryAttackDurationTimer.Start();
                CharacterData.IsSuppressed = true;
                return;
            case "ultimate_ability":
                return;
            default:
                GD.Print($"[WARN] Incorrect ENTER state: {state}");
                return;
        }
    }

    public override void ExitState(string state)
    {
        switch (state)
        {
            case "no_attack":
            case "main_attack":
            case "secondary_attack":
                CharacterData.IsSuppressed = false;
                return;
            case "ultimate_ability":
                return;
            default:
                GD.Print($"[WARN] Incorrect EXIT state: {state}");
                return;
        }
    }

    public override void ProcessState(string state, float direction, double delta)
    {
        // TODO: add ultimate ability lock behind XP level
        // TODO: add evolves? 
        switch (state)
        {
            case "no_attack":
                if (Input.IsActionJustPressed("main_attack"))
                {
                    ChangeState("main_attack");
                } else if (Input.IsActionJustPressed("secondary_attack") && CanSecondaryAttack())
                {
                    ChangeState("secondary_attack");
                } else if (Input.IsActionJustPressed("ultimate_ability"))
                {
                    ChangeState("ultimate_ability");
                }

                return;
            case "main_attack":
                if (Mathf.IsZeroApprox(_mainAttackDurationTimer.TimeLeft))
                {
                    ChangeState("no_attack");
                } else if (_mainAttackDurationTimer.TimeLeft < AttackBufferTime)
                {
                    if (Input.IsActionJustPressed("secondary_attack") && CanSecondaryAttack())
                    {
                        ChangeState("secondary_attack");
                    } else if (Input.IsActionJustPressed("ultimate_ability"))
                    {
                        ChangeState("ultimate_ability");
                    }
                }

                return;
            case "secondary_attack":
                if (Mathf.IsZeroApprox(_secondaryAttackDurationTimer.TimeLeft))
                {
                    ChangeState("no_attack");
                } else if (_secondaryAttackDurationTimer.TimeLeft < AttackBufferTime)
                {
                    if (Input.IsActionJustPressed("main_attack"))
                    {
                        ChangeState("main_attack");
                    } else if (Input.IsActionJustPressed("ultimate_ability"))
                    {
                        ChangeState("ultimate_ability");
                    }
                }
                
                return;
            case "ultimate_ability":
                return;
            default:
                GD.Print($"[WARN] Incorrect PROCESS state: {state}");
                return;
        }
    }
}