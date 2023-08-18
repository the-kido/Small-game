using System;
using Game.Players;
using Godot;

namespace Game.UI;

public sealed partial class PlayerClassMenu : ColorRect, IMenu {
    [Export]
    private Button close;

    public event Action Disable;

    public void Enable(Player player) {
        Visible = true;

        close.Pressed += OnDisable;
    }
    private void OnDisable() {
        close.Pressed -= OnDisable;
        Disable?.Invoke();
    }

    public void Close() {
        Visible = false;
    }
} 