using Godot;

namespace Threadcutter.Scripts;


public partial class FiniteStateMachineStateChange : RefCounted
{
    public string PrevState { get; set; }
    public string NextState { get; set; }
}


public partial class FiniteStateMachine : Node
{
    [Signal]
    public delegate void StateChangedEventHandler(FiniteStateMachineStateChange finiteStateMachineStateChange);
    
    public string CurrentState { get; private set; }
    
    [Export] public string InitialState { get; set; }

    public override void _Ready()
    {
        CurrentState = InitialState;
    }
    
    public virtual void EnterState(string state) {}
    public virtual void ExitState(string state) {}
    public virtual void ProcessState(string state, float direction, double delta) {}

    public void ProcessCurrentState(float direction, double delta)
    {
        ProcessState(CurrentState, direction, delta);
    }

    public void ChangeState(string nextState)
    {
        string prevState = CurrentState;
        
        ExitState(prevState);
        EnterState(nextState);

        CurrentState = nextState;

        EmitSignal(SignalName.StateChanged, new FiniteStateMachineStateChange()
        {
            PrevState = prevState, NextState = nextState
        });
    }
}