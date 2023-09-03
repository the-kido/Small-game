using System;
using Game.Players;
using Game.UI;
using Godot;

public class Settings {
}

public partial class SettingsPage : Control, IMenu {
    [Export]
    private Button closeButton;

    public event Action Disable;

    public override void _Ready() {
        closeButton.Pressed += () => Disable?.Invoke();
    }
    
    public void Enable(Player player) {
        Visible = true;
    }

    public void Close() {
        Visible = false;
    }
}