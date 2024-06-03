using System.Collections.Generic;
using Godot;

namespace Threadcutter.Components;



public partial class XpManager : Node
{
    [Signal]
    public delegate void GainedLevelEventHandler(int nextLevel);

    [Signal]
    public delegate void GainedXpEventHandler(int amount);

    public int CurrentLevel { get; private set; } = 1; // Always start at 1
    public int CurrentXp { get; private set; }

    // Level -> how much FULL XP is required to reach that level. For example, 3: 300 means that in order
    // to reath 3rd level, you need to have total of 300 XP (_current_xp >= 300)
    private readonly Dictionary<int, int> _xpRequiredToLevel = new()
    {
        { 2, 100 }, { 3, 300 }, { 4, 550 }, { 5, 900 }, { 6, 1500 }, { 7, 2200 }, { 8, 2900 },
        { 9, 3700 }, { 10, 4500 }
    };

    public void LevelUp()
    {
        int nextLevel = CurrentLevel + 1;
        CurrentLevel = nextLevel;

        EmitSignal(SignalName.GainedLevel, nextLevel);
    }

    public void GainXp(int amount)
    {
        CurrentXp += amount;

        if (CurrentLevel != 10)
        {
            int levelUpThreshold = _xpRequiredToLevel[CurrentLevel + 1];
            if (CurrentXp >= levelUpThreshold)
            {
                LevelUp();
            }
        } 
        
        EmitSignal(SignalName.GainedXp, amount);
    }
}