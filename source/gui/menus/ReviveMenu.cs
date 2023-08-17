using System;
using Godot;
using Game.Players;
/*This should include:
The enabling disableding
A way to set the menu to NULL in the playerHud 
*/
namespace Game.UI;

public partial class ReviveMenu : Control, IMenu{

    [Export]
    private AnimationPlayer animationPlayer;
    [Export]
    private Button close;

    public event Action Disable;
    public override void _Ready() {
        close.Pressed += () => Disable?.Invoke();
    }

    public void Enable(Player _) {
        Visible = true;
        animationPlayer.Play("Open");

        //Clear all methods the event is attached to.
        if (Disable is not null) {
            foreach (Delegate func in Disable.GetInvocationList()) {
                Disable -= func as Action;
            }
        }
    }

    public void Switch() {
        animationPlayer.Play("Open", -1, -1);
    }
}

