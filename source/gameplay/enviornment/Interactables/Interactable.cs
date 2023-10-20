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
            player.InputController.InteractablesButtonController.ClearInteractionInvokations();
    }

    private void OnBodyEntered(Node2D body) {
        if (body is Player)
            interactedWith.Add(this);
    }

    private void OnBodyExited(Node2D body) {
        if (body is Player) {
            interactedWith.Remove(this);
            DisableLingering();
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
    private static Interactable currentEnabledInteractable = null;
    static readonly List<Interactable> interactedWith = new(); 

    private void Enable(Player player, bool enable) {
        SetIndicatorVisibility(player, enable);
        AttachEvent(player, enable);
        
        if (enable) {
            currentEnabledInteractable = this;
        }
        if (!enable && interactedWith.Count is 0) {
            currentEnabledInteractable = null;  
        }
    }

    private void DisableLingering() {
        if (interactedWith.Count == 0) {
            currentEnabledInteractable?.Enable(Player.Players[0], false);
        }
    }
    // TODO
    private void DealWithMultipleThings() {
        Player player = Player.Players[0];

        Interactable closestInteractable = ClosestInteractable(player);
        
        if (closestInteractable is null || closestInteractable == currentEnabledInteractable) return;
        
        currentEnabledInteractable?.Enable(player, false);
        closestInteractable.Enable(player, true);
    }
    
    private static Interactable ClosestInteractable(Player player) {
        float closestDistance = float.MaxValue;
        Interactable closestInteractable = null;

        foreach (Interactable interactable in interactedWith) {
            float distance = interactable.GlobalPosition.DistanceTo(player.GlobalPosition);
            
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

    public void Destroy(Player player) {
            
        Interacted = null;
        interactedWith.Remove(this);
        player.InputController.InteractablesButtonController.ClearInteractionInvokations();

        currentEnabledInteractable?.Enable(player, false);
        currentEnabledInteractable = null;
        QueueFree();
    }
}



// what do we want:
/*
When  a player enters 1 area, it will enable that interactable
When a player is within 2 areas, it will start to check for which one is closer ONLY
When a player leaves an area and there's 1 area they're still within, that area is enabled ONLY
*/

/*
Maybe, instead, the process will deal with ALL interactable stuff

*/