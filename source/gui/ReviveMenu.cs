using Godot;

public partial class ReviveMenu : Control{

    [Export]
    private AnimationPlayer animationPlayer;

    public void Enable() {
        Visible = true;
        animationPlayer.Play("Open");
    }

}