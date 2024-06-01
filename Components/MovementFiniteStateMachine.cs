using Godot;
using Threadcutter.DataObjects;
using Threadcutter.Scripts;

namespace Threadcutter.Components;

public partial class MovementFiniteStateMachine : FiniteStateMachine
{
    [Export] public CharacterBody2D CharacterBody { get; set; }
    [Export] public VelocityComponent VelocityComponent { get; set; }
    

    [Export] public JumpData JumpData { get; set; }


    private Timer _coyoteJumpTimer;
    private Timer _jumpBufferTimer;

    public override void _Ready()
    {
        base._Ready();

        _coyoteJumpTimer = new Timer();
        _coyoteJumpTimer.WaitTime = 0.1f;
        _coyoteJumpTimer.OneShot = true;
        AddChild(_coyoteJumpTimer);
        
        _jumpBufferTimer = new Timer();
        _jumpBufferTimer.WaitTime = 0.1f;
        _jumpBufferTimer.OneShot = true;
        AddChild(_jumpBufferTimer);
    }


    public override void EnterState(string state)
    {
        switch (state)
        {
            case "idle":
                return;
            case "move":
                return;
            case "jump":
                VelocityComponent.Velocity = VelocityComponent.Velocity with { Y = JumpData.JumpVelocity };
                return;
            case "coyote_jump":
                return;
            case "fall":
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
            case "idle":
            case "move":
            case "jump":
            case "coyote_jump":
            case "fall":
                return;
            default:
                GD.Print($"[WARN] Incorrect EXIT state: {state}");
                return;
        }
    }
    
    public override void ProcessState(string state, float direction, double delta)
    {
        switch (state)
        {
            case "idle":
                if (CharacterBody.IsOnFloor())
                {
                    if (direction == 0.0f)
                    {
                        VelocityComponent.Velocity = VelocityComponent.Velocity with { X = 0.0f };
                    }

                    if (direction != 0.0f)
                    {
                        ChangeState("move");
                    }

                    if (Jumped())
                    {
                        ChangeState("jump");
                    }
                }

                return;
            case "move":
                if (CharacterBody.IsOnFloor())
                {
                    VelocityComponent.MoveInDirection(direction);
                    
                    if (Jumped())
                    {
                        ChangeState("jump");
                    }
                    
                    if (direction == 0.0f)
                    {
                        ChangeState("idle");
                    }
                }

                if (!CharacterBody.IsOnFloor() && VelocityComponent.Velocity.Y > 0.0f)
                {
                    _coyoteJumpTimer.Start();
                    ChangeState("coyote_jump");
                }

                return;
            case "jump":
                VelocityComponent.MoveInDirection(direction);

                if (!CharacterBody.IsOnFloor())
                {
                    float cutoffVelocity = JumpData.JumpVelocity * 0.1f; // Cutoff multiplier
                    if (Input.IsActionJustReleased("jump") && VelocityComponent.Velocity.Y < cutoffVelocity)
                    {
                        VelocityComponent.Velocity = VelocityComponent.Velocity with { Y = cutoffVelocity };
                    }

                    if (VelocityComponent.Velocity.Y > 0.0f)
                    {
                        ChangeState("fall");
                    }
                }

                return;
            case "coyote_jump":
                VelocityComponent.MoveInDirection(direction);

                if (!CharacterBody.IsOnFloor())
                {
                    if (Jumped())
                    {
                        ChangeState("jump");
                    }

                    bool coyoteWindowElapsed = _coyoteJumpTimer.TimeLeft == 0.0f;
                    if (VelocityComponent.Velocity.Y > 0.0f && coyoteWindowElapsed)
                    {
                        ChangeState("fall");
                    }
                }
                else
                {
                    ChangeState("idle");
                }

                return;
            case "fall":
                VelocityComponent.MoveInDirection(direction);

                if (CharacterBody.IsOnFloor())
                {
                    ChangeState(direction != 0.0f ? "move" : "idle");
                }

                return;
            default:
                GD.Print($"[WARN] Incorrect PROCESS state: {state}");
                return;
        }
    }

    private bool Jumped()
    {
        if (Input.IsActionJustPressed("jump"))
        {
            _jumpBufferTimer.Start();
            return false;
        }

        if (_jumpBufferTimer.TimeLeft > 0.0f)
        {
            _jumpBufferTimer.Stop();
            return true;
        }

        return false;
    } 
}