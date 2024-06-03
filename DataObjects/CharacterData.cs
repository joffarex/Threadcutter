using Godot;

namespace Threadcutter.DataObjects;

public partial class CharacterData : Resource
{
    [Export] public float CurrentDirection = 1.0f;
    [Export] public bool IsSuppressed { get; set; }

    [Export] private int _evolvePerLevelAmount;
    public int EvolvePerLevelAmount
    {
        get => _evolvePerLevelAmount;
        private set => _evolvePerLevelAmount = value;
    }

    [Export] private int _ultimateAbilityUnlockLevel;

    public int UltimateAbilityUnlockLevel
    {
        get => _ultimateAbilityUnlockLevel;
        private set => _ultimateAbilityUnlockLevel = value;
    }
}