using Godot;
using Threadcutter.Components;
using Threadcutter.DataObjects;
using Threadcutter.Entities.Characters;

namespace Threadcutter.Entities.BossAbilities;

public partial class BossAbilityDroppedItemData : DroppedItemData
{
    [Export] public PackedScene BossAbilityScene { get; set; }
    
    public override void ApplyItem(Character character)
    {
        character.BossAbilityManager.AssignBossAbilityScene(BossAbilityScene);
    }
}