using Godot;
using Threadcutter.Entities.Characters;

namespace Threadcutter.DataObjects;

public partial class DroppedItemData : Resource
{
    [Export] public string Name { get; set; }
    
    public virtual void ApplyItem(Character character) {}
}