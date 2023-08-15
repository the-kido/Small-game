using System;
using Godot;
using KidoUtils;

public partial class Interactable : AnimatedSprite2D {

    [Export]
    protected Area2D range;
    
    public event Action<Player> Interacted;

    public void SetIndicatorVisibility(Player player, bool isVisible) {
        player.GUI.InteractButton.Enable(isVisible);
        Visible = isVisible;
    }

    private void AttachEvent(Player player, bool attach) {
        if (attach)
            player.InputController.InteractablesButtonController.Interacted += Interacted;
        else
            player.InputController.InteractablesButtonController.Interacted -= Interacted;
    }

    // NOTE: This will 100% break when there are several players
    private void OnBodyEntered(Node2D body) {
        if (body is Player player) {
            SetIndicatorVisibility(player, true);
            AttachEvent(player, true);
        }
    }

    private void OnBodyExited(Node2D body) {
        if (body is Player player) {
            SetIndicatorVisibility(player, false);
            AttachEvent(player, false);
        }
    }

	public override void _Ready() {
        // Enforce the proper layers
        range.CollisionLayer = (uint) Layers.Environment;
        range.CollisionMask = (uint) Layers.Player;

        range.BodyEntered += OnBodyEntered;
        range.BodyExited += OnBodyExited;

        Visible = false;
	}
}

