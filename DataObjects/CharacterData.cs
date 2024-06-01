using Godot;

namespace Threadcutter.DataObjects;

public partial class CharacterData : Resource
{
    [Export] public float CurrentDirection = 1.0f;
    [Export] public bool IsSuppressed { get; set; }
}