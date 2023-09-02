using System;
using Game.Players;
using Godot;

namespace Game.UI;

public partial class EscapeMenu : Control, IMenu {
    [Export]
    private Button closeButton;

    public event Action Disable;

    public override void _Ready() => closeButton.Pressed += () => Disable?.Invoke();

    public void Close() {
        Visible = false;
        Disable = null;
    }

    public void Enable(Player player) {
        Visible = true;
    }
}