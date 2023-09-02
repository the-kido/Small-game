using Godot;
using System;
using System.Collections.Generic;
using KidoUtils;
using Game.Players.Mechanics;
using Game.UI;

namespace Game.Players.Inputs;

public class WeaponController {
	
	readonly WeaponManager hand;
	readonly HeldItemInputController heldItemInputController;

	public event Action<double> UseWeapon;
	public event Action<Vector2> UpdateWeaponDirection;
	public event Action OnWeaponLetGo;
	
	private bool isHoveringOverGui = false;
	private bool isAutoAttackButtonToggled = false;
	
	IPlayerAttackable targettedAttackable = null;
	readonly Player	player;
	private GUI GUI => player.GUI;

	readonly NodePath[] unclickableHUDElements = {"Auto Aim Button", "Dialogue Bar", "Interact", 
	"Held Weapons/1", "Held Weapons/2", "Held Weapons/3", "Held Weapons"};
    public WeaponController(WeaponManager hand, Player player) {
		this.hand = hand;
		this.player = player;

		heldItemInputController = new(hand);
		GUI.AttackButton.Pressed += () => isAutoAttackButtonToggled = !isAutoAttackButtonToggled;

		foreach (NodePath elementName in unclickableHUDElements) {
			Control element = (Control) GUI.HUD.GetNode(elementName); 
			
			element.MouseEntered += () => hoveringOverGui = true;
			element.MouseExited += () => hoveringOverGui = false;
		}
    }
	bool hoveringOverGui;

	public void UnsubEvents() {
		UpdateWeaponDirection = null;
		UseWeapon = null;
		OnWeaponLetGo = null;
	}
	
	private List<InputType> GetAttackInputs() {
		List<InputType> inputMap = new();

		if (isAutoAttackButtonToggled)
			inputMap.Add(InputType.AutoAttackButtonToggled);

		if (Input.IsActionJustPressed("utility_used"))
			inputMap.Add(InputType.RightClickJustPressed);
		
		if (Input.IsActionPressed("default_attack") && !hoveringOverGui) 
			inputMap.Add(InputType.LeftClickHold);

		if (Input.IsActionJustReleased("default_attack") && !hoveringOverGui)
			inputMap.Add(InputType.LeftClickJustReleased);

		return inputMap;
	}

    public IPlayerAttackable FindInteractableWithinCursor() {
		List<Node2D> list = Utils.GetPreloadedScene<GlobalCursor>(hand, PreloadedScene.GlobalCursor).ObjectsInCursorRange;

        foreach (Node2D node2d in list) {
            if (node2d is IPlayerAttackable attackable) {
                return attackable;
            }
        }
        return null;
    }

    private bool IsInteractableVisible(IPlayerAttackable interactable) {
        PhysicsDirectSpaceState2D spaceState = hand.GetWorld2D().DirectSpaceState;
        var ray = PhysicsRayQueryParameters2D.Create(hand.GlobalPosition, interactable.GetPosition(), (uint) Layers.Environment);
        var result = spaceState.IntersectRay(ray);
        //if nothing hit the ray, then we good. OR, if the ray hit the interactable, we're also good.
        return result.Count <= 0 || (Rid) result["collider"] == interactable.GetRid();
    }
 
	readonly uint faceObjectMask = (uint) Layers.Environment + (uint) Layers.Enemies;
    private IPlayerAttackable FindObjectToFace(List<IPlayerAttackable> _) {
        //Check if the player is clicking/pressing on the screen. 
        foreach (IPlayerAttackable playerAttactable in Player.Players[0].InteractableRadar.NearbyEnemies) {

            PhysicsDirectSpaceState2D spaceState = hand.GetWorld2D().DirectSpaceState;
            var ray = PhysicsRayQueryParameters2D.Create(hand.GlobalPosition, playerAttactable.GetNode().GlobalPosition, faceObjectMask);
            var result = spaceState.IntersectRay(ray);

            if (result.Count > 0 && (Rid) result["collider"] == playerAttactable.GetRid())
                return playerAttactable;
        }
        return null;
    }

	enum ControlMethod { Autoaim, SelectedAutoaim, ManualAim }

	private ControlMethod useMethod; 

	private ControlMethod GetUseMethod(IPlayerAttackable targettedInteractable) {
		List<InputType> inputMap = GetAttackInputs();

		// If something happened to the interactable, default to default aim method.
        if (useMethod is ControlMethod.SelectedAutoaim && targettedInteractable is null) return ControlMethod.ManualAim;

		if (inputMap.Contains(InputType.AutoAttackButtonToggled)) return ControlMethod.Autoaim;

		// This is put last for most priority. If the player explicitly wants to target a unit, it should 
		if (targettedInteractable is not null && targettedInteractable.IsInteractable()) return ControlMethod.SelectedAutoaim;
		
		// Default to manual aim
		return ControlMethod.ManualAim;
	}
	
    public void Update(double delta) {
		heldItemInputController.Update();

		if (hand.HeldWeapon is null)
			return;
		
		List<InputType> inputMap = GetAttackInputs();

		if (inputMap.Contains(InputType.LeftClickJustReleased)) 
			OnWeaponLetGo?.Invoke();

		if (inputMap.Contains(InputType.RightClickJustPressed)) {
            targettedAttackable = FindInteractableWithinCursor();
            
            if (targettedAttackable is null)
                SelectedTargetIndicator.RemoveDuplicate();
			else
				GUI.TargetIndicator.Enable(targettedAttackable);
		}
		
		useMethod = GetUseMethod(targettedAttackable);

		switch (useMethod) {
			case ControlMethod.SelectedAutoaim:
				//If using a hold-to-charge weapon, the charge should increase when not looking at thing.
				if (hand.HeldWeapon.WeaponType is Weapon.Type.HoldToCharge) {
                    UpdateWeaponDirection?.Invoke(targettedAttackable.GetPosition());
					UseWeapon?.Invoke(delta);
					break;
				}
                GD.Print("this is work");

				//If the interactable is still in tact and is still visible, autoshoot it.
				if (IsInteractableVisible(targettedAttackable)) {
                    GD.Print("Visible");
					UpdateWeaponDirection?.Invoke(targettedAttackable.GetPosition());
					UseWeapon?.Invoke(delta);
				}
				break;

			case ControlMethod.Autoaim:
                IPlayerAttackable see = FindObjectToFace(Player.Players[0].InteractableRadar.NearbyEnemies);
				
				if (hand.HeldWeapon.WeaponType is Weapon.Type.HoldToCharge) {
					UseWeapon?.Invoke(delta); 
				}

				if (see is not null) {
					UpdateWeaponDirection?.Invoke(see.GetNode().GlobalPosition);
					//This is literally the only solution i can  think of..
					if (hand.HeldWeapon.WeaponType is not Weapon.Type.HoldToCharge)
						UseWeapon?.Invoke(delta);
				}
				break;

			case ControlMethod.ManualAim:
				if (isHoveringOverGui) break;
				//If the player is aiming themselves, shoot where they're pointing.

				if (inputMap.Contains(InputType.LeftClickHold))
					UseWeapon?.Invoke(delta);
				
				//Default the weapon to point to the cursor.
				UpdateWeaponDirection?.Invoke(hand.GetGlobalMousePosition());
				break;
		}
	}
}
