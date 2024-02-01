using Godot;

public partial class InteractableDescription : Control {
    [Export]
    RichTextLabel textLabel;
    [Export]
    AnimationPlayer animationPlayer;

    public override void _Ready() {
        textLabel.Text = null;
        Visible = false;
    }

    public void Disable() {
        if (string.IsNullOrEmpty(textLabel.Text)) return;
        animationPlayer.Play("Close");
    }

    public void Enable(string description) {
        textLabel.Text = description;
        animationPlayer.Play("Open");
    }
}