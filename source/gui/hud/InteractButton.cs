using Godot;

public partial class InteractButton : Button {
    [Export]
    AnimationPlayer animationPlayer;
    public void Enable(bool enable) {
        Visible = enable;
        
        if (enable)
            animationPlayer.Play("show");
        else
            animationPlayer.PlayBackwards("show");
    }
}