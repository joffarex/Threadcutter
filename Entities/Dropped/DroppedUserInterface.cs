using Godot;

namespace Threadcutter.Entities.Dropped;

public partial class DroppedUserInterface : Control
{
    [Export] public string InteractLabelText { get; set; }

    public Label InteractLabel { get; private set; }

    public override void _Ready()
    {
        InteractLabel = GetNode<Label>("HBoxContainer/InteractLabel");
        UpdateInteractLabelText(InteractLabelText);
        InteractLabel.Hide();
    }

    public void UpdateInteractLabelText(string incoming)
    {
        InteractLabel.Text = incoming;
    }
}