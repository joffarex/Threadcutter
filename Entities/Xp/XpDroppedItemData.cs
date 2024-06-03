using Godot;
using Threadcutter.DataObjects;
using Threadcutter.Entities.Characters;

namespace Threadcutter.Entities.Xp;

public partial class XpDroppedItemData : DroppedItemData
{
    [Export] public int XpValue { get; set; }
    
    
    public override void ApplyItem(Character character)
    {
        character.XpManager.GainXp(XpValue);
    }
}