using System;
using Godot;
using KidoUtils;
using Game.Players;
using System.Collections.Generic;

namespace Game.LevelContent;

public sealed partial class Interactable : AnimatedSprite2D {

    [Export]
    private Area2D range;
    
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


    // Currently does not support many players. Or does it?
    static readonly List<Interactable> interactedWith = new();
    private void OnBodyEntered(Node2D body) {
        if (body is Player player) {
            interactedWith.Add(this);
            Enable(player, true);
        }
    }
    private void Enable(Player player, bool enable) {
        SetIndicatorVisibility(player, enable);
        AttachEvent(player, enable);
    }

    private void OnBodyExited(Node2D body) {
        if (body is Player player) {
            interactedWith.Remove(this);
            Enable(player, false);
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

