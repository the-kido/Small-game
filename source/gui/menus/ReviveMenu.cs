using System;
using Godot;
using Game.Players;
using Game.Autoload;
using KidoUtils;
using Game.Data;
using Game.LevelContent;

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
        close.Pressed += Die;
        respawn.Pressed += Respawn;
    }
    
    private void Respawn() {
        OnDisable();
        Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPath(Level.CurrentScenePath);
        RunData.RespawnTokens.Add(-1);
        GameDataService.Save();
    }

    const string spawnScene = "res://assets/levels/debug/spawn.tscn";
    private void Die() {
        Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPath(spawnScene);
        OnDisable();
    }

    private void OnDisable() {
        Disable?.Invoke();
    }

    public void Enable(Player _) {
        Visible = true;
        animationPlayer.Play("Open");
        
        int tokenCount = RunData.RespawnTokens.Count;

        respawn.Text = tokenCount > 0 
            ? $"You have {tokenCount} respawn token" + (tokenCount > 1 ? "s" : "") 
            : "You have no respawn tokens left";

        respawn.Disabled = tokenCount <= 0;

        Disable = null;
    }

    public void Close() {
        animationPlayer.PlayBackwards("Open");
        animationPlayer.AnimationFinished += OnMenuClosed; 
    }

    private void OnMenuClosed(StringName _) {
        Visible = false;
        animationPlayer.AnimationFinished -= OnMenuClosed; 
    }
}

