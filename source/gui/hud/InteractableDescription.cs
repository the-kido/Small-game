using Godot;

public partial class InteractableDescription : Control {
    [Export]
    RichTextLabel textLabel;
    [Export]
    AnimationPlayer animationPlayer;

    public void Enable(bool @bool, string description) {
        // Visible = @bool;
        textLabel.Text = description;

        animationPlayer.Play(@bool ? "Open" : "Close");
    }
}