using System;
using Game.Players;
using Godot;

public partial class HoverButton : ColorRect {
    [Export]
    Control controlShownOnHover;

    public event Action SelectionMade;

    public void Init(Player player) {
        player.InputController.LeftClicked += OnLeftClicked;
    }

    public void Close(Player player) {
        player.InputController.LeftClicked -= OnLeftClicked;
    }

    public override void _Ready() {
        MouseEntered += ShowHoverControl;
        MouseExited += HideHoverControl;
    }

    private void OnLeftClicked() {
        // For PC
        string osName = OS.GetName();
        
        if (showingControl)
            SelectionMade?.Invoke();
    }

    public override void _Process(double delta) {

        if (showingControl) {
            controlShownOnHover.GlobalPosition = GetGlobalMousePosition();
        }
    }

    bool showingControl = false;
    private void ShowHoverControl() {
        showingControl = true;
    }
    private void HideHoverControl() {
        showingControl = false;
    }
}