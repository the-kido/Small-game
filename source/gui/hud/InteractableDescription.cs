using Godot;

public partial class InteractableDescription : Control {
    [Export]
    RichTextLabel textLabel;
    [Export]
    AnimationPlayer animationPlayer;

    public override void _Ready() => textLabel.Text = null;
    

    public void Enable(bool @bool, string description) {
        if (!@bool && string.IsNullOrEmpty(textLabel.Text)) return;

        textLabel.Text = description;

        animationPlayer.Play(@bool ? "Open" : "Close");
    }
}