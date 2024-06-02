using System;
using Godot;
using Threadcutter.Entities;
using Threadcutter.Entities.Characters;
using Threadcutter.Entities.Dropped;

namespace Threadcutter.Components;

public partial class DroppedEntityManager : Node2D
{
    [Export] public Character Character { get; set; }
    
    // Manuals
    private Area2D DroppedAbilityPickup { get; set; }
    
    // Auto pickups
    private Area2D DroppedGemPickup { get; set; }
    private Area2D DroppedXpPickup { get; set; }

    public override void _Ready()
    {
        DroppedAbilityPickup = GetNode<Area2D>("DroppedAbilityPickup");
        DroppedGemPickup = GetNode<Area2D>("DroppedGemPickup");
        DroppedXpPickup = GetNode<Area2D>("DroppedXpPickup");
        
        DroppedGemPickup.AreaEntered += OnDroppedAutoPickupRangeEntered;
        DroppedXpPickup.AreaEntered += OnDroppedAutoPickupRangeEntered;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("interact"))
        {
            bool picked = false;
            
            if (DroppedAbilityPickup.HasOverlappingAreas())
            {
                var overlappingAreas = DroppedAbilityPickup.GetOverlappingAreas();

                foreach (var overlappingArea in overlappingAreas)
                {
                    if (picked)
                    {
                        break;
                    }

                    if (overlappingArea is Dropped dropped)
                    {
                        dropped.DroppedItemData.ApplyItem(Character);
                        dropped.QueueFree();
                        picked = true;
                    }
                }
            }
        }
    }
    
    private void OnDroppedAutoPickupRangeEntered(Area2D area)
    {
        if (area is Dropped dropped)
        {
            var moveTowardsCharacterTimer = new Timer();
            moveTowardsCharacterTimer.OneShot = false;
            moveTowardsCharacterTimer.Autostart = false;
            moveTowardsCharacterTimer.WaitTime = 0.01f;
            moveTowardsCharacterTimer.Timeout += () =>
            {
                var directionToCharacter = dropped.Position.DirectionTo(Character.Position).Normalized();
                dropped.Position += directionToCharacter * 2;

                (bool inXRange, bool inYRange) inRange = GetRangeTowardsCharacter(dropped.GlobalPosition);
                if (inRange is { inXRange: true, inYRange: true })
                {
                    dropped.DroppedItemData.ApplyItem(Character);
                    dropped.QueueFree();
                }
            };
            dropped.AddChild(moveTowardsCharacterTimer);
            moveTowardsCharacterTimer.Start();
        }
    }
    
    private (bool, bool) GetRangeTowardsCharacter(Vector2 fromGlobalPosition)
    {
        const int range = 10;
    
        int characterX = (int)Math.Round(Character.GlobalPosition.X);
        int characterY = (int)Math.Round(Character.GlobalPosition.Y);

        int roundedFromX = (int)Math.Round(fromGlobalPosition.X);
        int roundedFromY = (int)Math.Round(fromGlobalPosition.Y);

        bool inXRange = characterX - range <= roundedFromX && roundedFromX <= characterX + range;
        bool inYRange = characterY - range <= roundedFromY && roundedFromY <= characterY + range;

        return new ValueTuple<bool, bool>(inXRange, inYRange);
    }
}