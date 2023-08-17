using Godot;

namespace Game.UI;

public partial class HUDCover : ColorRect {
    [Export]
    private AnimationPlayer animationPlayer;

    public void Enable(bool enable) {
        Visible = enable;
        
        if (enable)
            animationPlayer.Play("enable");
        else
            animationPlayer.Play("enable");
    }
}