using Godot;

namespace Threadcutter.Components;

public partial class BossAbilityManager : Node
{
    [Export] public PackedScene AssignedBossAbilityScene { get; set; }
    [Export] public CharacterBody2D CharacterBody { get; set; }

    private Node2D _abilityContainer;

    public override void _Ready()
    {
        _abilityContainer = GetTree().GetFirstNodeInGroup("AbilityContainer") as Node2D;
    }


    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("boss_ability"))
        {
            if (IsInstanceValid(AssignedBossAbilityScene))
            {
                if (AssignedBossAbilityScene.Instantiate() is Node2D bossAbilityInstance)
                {
                    bossAbilityInstance.GlobalPosition = CharacterBody.GlobalPosition;
                    _abilityContainer.AddChild(bossAbilityInstance);
                }
            }
        }
    }

    public void AssignBossAbilityScene(PackedScene incomingScene)
    {
        AssignedBossAbilityScene = incomingScene;
    }
}