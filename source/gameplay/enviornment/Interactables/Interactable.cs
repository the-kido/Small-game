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
    
    private void Enable(Player player, bool enable) {
        if (enable)
            interactedWith.Add(this);
        else 
            interactedWith.Remove(this);
        
        if (interactedWith.Count > 1) {
            return; // The _process method will deal with surplus interactables;
        }

        SetIndicatorVisibility(player, enable);
        AttachEvent(player, enable);

        if (enable) {
            currentEnabledInteractable = this;
        }
    }

    static Interactable currentEnabledInteractable = null;

    private void OnBodyEntered(Node2D body) {
        if (body is Player player) {
            Enable(player, true);
        }
    }

    private void OnBodyExited(Node2D body) {
        if (body is Player player) {
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

    // NOTE: This will 100% break when there are several players
    // Currently does not support many players. Or does it?
    static readonly List<Interactable> interactedWith = new(); 


    // TODO
    
    private void DealWithMultipleThings() {
        if (interactedWith.Count <= 1) return;

        Player player = Player.Players[0];

        Interactable closestInteractable = ClosestInteractable(player);
        
        if (closestInteractable == currentEnabledInteractable) return;

        for (int i = 0; i < interactedWith.Count; i++) {
            Interactable temp = interactedWith[i];
            temp.Enable(player, false);
        }
        
        closestInteractable.Enable(player, true);

    }
    
    private Interactable ClosestInteractable(Player player) {
        float closestDistance = float.MaxValue;
        Interactable closestInteractable = null;

        foreach (Interactable interactable in interactedWith) {
            float distance = interactable.GlobalPosition.DistanceTo(Player.Players[0].GlobalPosition);
            
            if (distance < closestDistance) {
                closestDistance = distance;
                closestInteractable = interactable;
            } 
        }

        return closestInteractable;
    }
    
    public override void _Process(double delta) {
        DealWithMultipleThings();
    }
}

