using System;
using Godot;
using Game.Players;
using Game.Autoload;
using KidoUtils;
using Game.Data;
using System.Reflection;
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
    [Export]
    private Button respawn;

    public Action Disable {get; set;}
    public override void _Ready() {
        close.Pressed += OnDisable;
        // respawn.Pressed
    } 

    private void OnDisable() {
        close.Pressed -= OnDisable; 
        Disable?.Invoke();
        Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPath("res://assets/levels/debug/spawn.tscn");
    }

    public void Enable(Player _) {
        Visible = true;
        animationPlayer.Play("Open");
        
        int tokenCount = RunData.AllData[RunDataEnum.RespawnTokens].Count;

        respawn.Text = tokenCount > 0 
            ? $"You have {tokenCount} respawn token" + (tokenCount > 1 ? "s" : "") 
            : "You have no respawn tokens left";

        respawn.Disabled = tokenCount <= 0;

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

