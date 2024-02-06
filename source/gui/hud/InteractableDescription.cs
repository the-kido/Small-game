using Godot;

public partial class InteractableDescription : Control {
    [Export]
    RichTextLabel textLabel;
    [Export]
    AnimationPlayer animationPlayer;
    
    public override void _Ready() {
        textLabel.Text = null;
    }

    public void Disable() {
        if (string.IsNullOrEmpty(textLabel.Text)) return;
        animationPlayer.PlayBackwards("Open");
    }

    public void Enable(string description) {
        animationPlayer.Play("Open");
        textLabel.Text = description;
    }
}