using Godot;
using Threadcutter.DataObjects;
using Threadcutter.Entities.Characters;

namespace Threadcutter.Entities.Dropped;

public partial class Dropped : Area2D
{
    [Export] public string DroppedItemInteractLabelText { get; set; }
    [Export] public Texture2D DroppedItemTexture { get; set; }
    [Export] public DroppedItemData DroppedItemData { get; set; }
    [Export] public bool AutoPickup { get; set; } = true;

    private DroppedUserInterface _droppedUserInterface;
    private Sprite2D _sprite;
    private Area2D _interactArea;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _sprite.Texture = DroppedItemTexture;
        
        _droppedUserInterface = GetNode<DroppedUserInterface>("DroppedUserInterface");
        _droppedUserInterface.UpdateInteractLabelText(DroppedItemInteractLabelText);

        _interactArea = GetNode<Area2D>("InteractArea");
        _interactArea.BodyEntered += body =>
        {
            if (body is Character)
            {
                _droppedUserInterface.InteractLabel.Show();
            }
        };
        _interactArea.BodyExited += body =>
        {

            if (body is Character)
            {
                _droppedUserInterface.InteractLabel.Hide();
            }
        };
    }
}