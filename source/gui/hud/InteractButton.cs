using Godot;

public partial class InteractButton : Button {
    [Export]
    AnimationPlayer animationPlayer;

    
    public void Enable(bool enable) {
        Disabled = !enable;

        if (enable) {
            Visible = enable;
            animationPlayer.Play("show");
        }
        else {
            animationPlayer.PlayBackwards("show");
            animationPlayer.AnimationFinished += Hide;
            
        }
    }

    private void Hide(StringName _) {
        Visible = false;
        animationPlayer.AnimationFinished -= Hide;
    }
}