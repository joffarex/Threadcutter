using Godot;
using Threadcutter.DataObjects;

namespace Threadcutter.Entities;

public partial class Dropped : Area2D
{
    [Export] public string DroppedLabel { get; set; }
    [Export] public Sprite2D DroppedSprite { get; set; }
    [Export] public DroppedItemData DroppedItemData { get; set; }
    [Export] public bool AutoPickup { get; set; } = true;
}