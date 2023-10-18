using System;
using Game.Autoload;
using Game.Players;
using Godot;
using KidoUtils;

namespace Game.UI;

public partial class EscapeMenu : Control, IMenu {
    [Export]
    private Button closeButton;
    [Export]
    private AnimationPlayer animationPlayer;
    [Export]
    private Button goToMainMenu;
    [Export]
    private Button settingsButton;

    public Action Disable {get; set;}
    public override void _Ready() {
        
        closeButton.Pressed += () => Disable?.Invoke();
        settingsButton.Pressed += () => Utils.GetPreloadedScene<GUI>(this, PreloadedScene.GUI).OpenSettingsPage(); 

        goToMainMenu.Pressed += () => {
            Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPath("res://source/gui/MainMenu.tscn");
            Disable?.Invoke();
        };
    }

    public void Close() {
        animationPlayer.Play("Close");
        Disable = null;
    }

    public void Enable(Player player) {
        animationPlayer.Play("Open");
    }
}