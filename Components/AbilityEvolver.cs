using System.Collections.Generic;
using Godot;
using Threadcutter.DataObjects;

namespace Threadcutter.Components;

public enum AbilityEvolveLevel
{
    Zero, One, Two
}

public partial class AbilityEvolvedData : RefCounted
{
    public string Ability { get; set; }
    public AbilityEvolveLevel EvolveLevel { get; set; }
}


public partial class AbilityEvolver : Node
{
    [Signal]
    public delegate void AbilityEvolvedEventHandler(AbilityEvolvedData abilityEvolvedData);
    
    
    private readonly Godot.Collections.Dictionary<string, AbilityEvolveLevel> _currentEvolveLevels =
        new()
        {
            { "main_attack", AbilityEvolveLevel.Zero },
            { "secondary_attack", AbilityEvolveLevel.Zero },
            { "ultimate_ability", AbilityEvolveLevel.Zero },
            { "movement_ability", AbilityEvolveLevel.Zero },
        };
    
    [Export] public CharacterData CharacterData { get; set; }
    [Export] public XpManager XpManager { get; set; }
    
    public AbilityEvolveLevel GetAbilityEvolveLevel(string abilityName)
    {
        return _currentEvolveLevels.GetValueOrDefault(abilityName, AbilityEvolveLevel.Zero);
    }

    public void EvolveAbilityLevel(string abilityName)
    {
        if (_currentEvolveLevels.TryGetValue(abilityName, out AbilityEvolveLevel abilityEvolveLevel))
        {
            switch (abilityEvolveLevel)
            {
                case AbilityEvolveLevel.Zero:
                    _currentEvolveLevels[abilityName] = AbilityEvolveLevel.One;
                    EmitSignal(SignalName.AbilityEvolved,
                        new AbilityEvolvedData()
                            { Ability = abilityName, EvolveLevel = _currentEvolveLevels[abilityName] });
                    return;
                case AbilityEvolveLevel.One:
                    _currentEvolveLevels[abilityName] = AbilityEvolveLevel.Two;
                    EmitSignal(SignalName.AbilityEvolved,
                        new AbilityEvolvedData()
                            { Ability = abilityName, EvolveLevel = _currentEvolveLevels[abilityName] });
                    return;
                case AbilityEvolveLevel.Two:
                    return;
                default:
                    GD.Print($"[WARN] incorrect ability evolve level set on {abilityName}");
                    return;
            }
        }
    }

    public string ConstructEvolvedAbilityAlias(string abilityName)
    {
        if (_currentEvolveLevels.TryGetValue(abilityName, out AbilityEvolveLevel abilityEvolveLevel))
        {
            switch (abilityEvolveLevel)
            {
                case AbilityEvolveLevel.Zero:
                    return abilityName;
                case AbilityEvolveLevel.One:
                case AbilityEvolveLevel.Two:
                    return $"{abilityName}_${abilityEvolveLevel.ToString()}";
                default:
                    GD.Print($"[WARN] incorrect ability evolve level set on {abilityName}");
                    return abilityName;
            }
        }

        GD.Print($"[WARN] incorrect ability name passed: ${abilityName}");
        return abilityName;
    }

    public (string, string) SuggestAbilityEvolves()
    {
        var abilities = new List<string>(){ "main_attack", "secondary_attack", "movement_ability" };

        if (CharacterData.UltimateAbilityUnlockLevel <= XpManager.CurrentLevel)
        {
            abilities.Add("ultimate_ability");
        }
        
        // NOTE: This indexing is a bit scuffed. It depends on the ability names being in order in both the array
        // and in the _current_evolve_levels
        var randomizer = new RandomNumberGenerator();
        int randomIndexOne = randomizer.RandiRange(0, abilities.Count - 1);
        int randomIndexTwo = randomizer.RandiRange(0, abilities.Count - 1);

        while (randomIndexTwo == randomIndexOne)
        {
            randomIndexTwo = randomizer.RandiRange(0, abilities.Count - 1);
        }

        return (abilities[randomIndexOne], abilities[randomIndexTwo]);
    }
}