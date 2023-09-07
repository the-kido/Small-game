using System;
using Godot;
using Game.Players;
using Game.Autoload;
using KidoUtils;
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
    public override void _Ready() => close.Pressed += OnDisable; 

    private void OnDisable() {
        close.Pressed -= OnDisable; 
        Disable?.Invoke();
        Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPath("res://assets/levels/debug/spawn.tscn");
    }

    public void Enable(Player _) {
        Visible = true;
        animationPlayer.Play("Open");

        //Clear all methods the event is attached to.
        Disable = null;
    }

    public void Close() {
        animationPlayer.PlayBackwards("Open");
        animationPlayer.AnimationFinished += OnClosed; 
    }

    private void OnClosed(StringName _) {
        Visible = false;
        animationPlayer.AnimationFinished -= OnClosed; 
    }
}

