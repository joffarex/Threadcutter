using Godot;

namespace Threadcutter.Components;

public partial class HealthChange : RefCounted
{
    public int Prev { get; set; }
    public int Next { get; set; }
    public bool IsHeal { get; set; }
}


public partial class HealthComponent : Node
{
    private int _currentHealth;

    [Signal]
    public delegate void HealthChangedEventHandler(HealthChange healthChange);

    [Signal]
    public delegate void DiedEventHandler();
    
    [Export] public int InitialHealth { get; set; }
    [Export] public int MaxHealth { get; set; }


    public int CurrentHealth
    {
        get => _currentHealth;
        private set
        {
            int prevHealth = _currentHealth;
            int nextHealth;

            if (value < 0)
            {
                nextHealth = 0;
            }
            else if (value > MaxHealth)
            {
                nextHealth = MaxHealth;
            }
            else
            {
                nextHealth = value;
            }

            _currentHealth = nextHealth;
            EmitSignal(SignalName.HealthChanged, new HealthChange()
            {
                Prev = prevHealth,
                Next = nextHealth,
                IsHeal = _currentHealth > prevHealth
            });
            
            if (_currentHealth <= 0)
            {
                EmitSignal(SignalName.Died);
            }
        }
    }

    public override void _Ready()
    {
        CurrentHealth = InitialHealth;
    }
    
    public void AddHealth(int amount)
    {
        CurrentHealth += amount;
    }

    public void RemoveHealth(int amount)
    {
        CurrentHealth -= amount;
    }
}