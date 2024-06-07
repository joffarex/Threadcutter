using Godot;
using Threadcutter.DataObjects;
using Threadcutter.Scripts;

namespace Threadcutter.Components;

public partial class CharacterCombatFiniteStateMachine : FiniteStateMachine
{
    [Export] public CharacterBody2D CharacterBody { get; set; }
    [Export] public CharacterData CharacterData { get; set; }
    [Export] public AbilityEvolver AbilityEvolver { get; set; }
    [Export] public CharacterMovementFiniteStateMachine CharacterMovementFiniteStateMachine { get; set; }

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

        AbilityEvolver.AbilityEvolved += abilityEvolvedData =>
        {
            GD.Print($"Successfully evolved: {abilityEvolvedData.Ability} | {abilityEvolvedData.EvolveLevel}");
            // TODO: get new animation from animation player and update wait time on attack durations
        };
    }

    private bool CanSecondaryAttack() =>
        CharacterMovementFiniteStateMachine.CurrentState != "jump" &&
        CharacterMovementFiniteStateMachine.CurrentState != "fall" &&
        CharacterMovementFiniteStateMachine.CurrentState != "coyote_jump";
    public override void EnterState(string state)
    {
        switch (state)
        {
            case "no_attack":
                return;
            case "main_attack":
            case "main_attack_one":
            case "main_attack_two":
                _mainAttackDurationTimer.Start();
                return;
            case "secondary_attack":
            case "secondary_attack_one":
            case "secondary_attack_two":
                _secondaryAttackDurationTimer.Start();
                CharacterData.IsSuppressed = true;
                return;
            case "ultimate_ability":
            case "ultimate_ability_one":
            case "ultimate_ability_two":
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
            case "main_attack_one":
            case "main_attack_two":
                return;
            case "secondary_attack":
            case "secondary_attack_one":
            case "secondary_attack_two":
                CharacterData.IsSuppressed = false;
                return;
            case "ultimate_ability":
            case "ultimate_ability_one":
            case "ultimate_ability_two":
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
                    ChangeState(AbilityEvolver.ConstructEvolvedAbilityAlias("main_attack"));
                } else if (Input.IsActionJustPressed("secondary_attack") && CanSecondaryAttack())
                {
                    ChangeState(AbilityEvolver.ConstructEvolvedAbilityAlias("secondary_attack"));
                } else if (Input.IsActionJustPressed("ultimate_ability"))
                {
                    ChangeState(AbilityEvolver.ConstructEvolvedAbilityAlias("ultimate_ability"));
                }

                return;
            case "main_attack":
            case "main_attack_one":
            case "main_attack_two":
                MainAttack();

                return;
            case "secondary_attack":
            case "secondary_attack_one":
            case "secondary_attack_two":
                SecondaryAttack();
                
                return;
            case "ultimate_ability":
            case "ultimate_ability_one":
            case "ultimate_ability_two":
                return;
            default:
                GD.Print($"[WARN] Incorrect PROCESS state: {state}");
                return;
        }
    }

    private void MainAttack()
    {
        if (Mathf.IsZeroApprox(_mainAttackDurationTimer.TimeLeft))
        {
            ChangeState("no_attack");
        } else if (_mainAttackDurationTimer.TimeLeft < AttackBufferTime)
        {
            if (Input.IsActionJustPressed("secondary_attack") && CanSecondaryAttack())
            {
                ChangeState(AbilityEvolver.ConstructEvolvedAbilityAlias("secondary_attack"));
            } else if (Input.IsActionJustPressed("ultimate_ability"))
            {
                ChangeState(AbilityEvolver.ConstructEvolvedAbilityAlias("ultimate_ability"));
            }
        }
    }

    private void SecondaryAttack()
    {
        if (Mathf.IsZeroApprox(_secondaryAttackDurationTimer.TimeLeft))
        {
            ChangeState("no_attack");
        } else if (_secondaryAttackDurationTimer.TimeLeft < AttackBufferTime)
        {
            if (Input.IsActionJustPressed("main_attack"))
            {
                ChangeState(AbilityEvolver.ConstructEvolvedAbilityAlias("main_attack"));
            } else if (Input.IsActionJustPressed("ultimate_ability"))
            {
                ChangeState(AbilityEvolver.ConstructEvolvedAbilityAlias("ultimate_ability"));
            }
        }
    }
}