using Godot;

namespace Game.UI;

public partial class InteractButton : Button {
    [Export]
    AnimationPlayer animationPlayer;
    
    public void Enable(bool enable) {

        if (enable) {
            Visible = true;
            animationPlayer.Play("show");
        }
        else {
            if (connected) return;

            animationPlayer.PlayBackwards("show");
            animationPlayer.AnimationFinished += Hide;
            connected = true;
        }
    }
    bool connected;

    private void Hide(StringName _) {
        connected = false;

        animationPlayer.AnimationFinished -= Hide;
        
        if (!Disabled) 
            return;
        
        Visible = false;
    }
}