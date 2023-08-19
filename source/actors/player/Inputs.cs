using Godot;
using System;
using System.Collections.Generic;
using KidoUtils;
using Game.Players.Mechanics;
using Game.Actors;
using Game.UI;

namespace Game.Players.Inputs;

public class ShieldInput  {
	readonly Player player;

	public ShieldInput(Player player) {
		this.player = player;
	}
	public event Action<bool> PlayerShieldsDamage;
	bool cache; 
	public void Update() {
		if (cache != IsBlockingDamage()) {
			cache = IsBlockingDamage();
			player.DamageableComponent.BlocksDamage = IsBlockingDamage();
			PlayerShieldsDamage?.Invoke(cache);
		}
	}

	private bool IsBlockingDamage() {
		if (player.ShieldManager.HeldShield is null) 
			return false;

		if (!player.ShieldManager.HeldShield.Alive)
			return false;
		
		if (Input.IsActionPressed("utility_used"))
			return true;
		
		return false;
	}
}

public class HeldItemInputController {
	readonly WeaponManager hand;
	public HeldItemInputController(WeaponManager hand) {
		this.hand = hand;
	}

	public void Update() {
		if (Input.IsActionJustPressed("select_slot_1"))
			hand.SwitchHeldWeapon(0);

		if (Input.IsActionJustPressed("select_slot_2"))
			hand.SwitchHeldWeapon(1);
		
		if (Input.IsActionJustPressed("select_slot_3"))
			hand.SwitchHeldWeapon(2);
	}
}

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
    public WeaponController(WeaponManager hand, Player player) {
		this.hand = hand;
		this.player = player;

		heldItemInputController = new(hand);
		GUI.AttackButton.Pressed += () => isAutoAttackButtonToggled = !isAutoAttackButtonToggled;
    }

	private List<InputType> GetAttackInputs() {
		List<InputType> inputMap = new();

		if (isAutoAttackButtonToggled)
			inputMap.Add(InputType.AutoAttackButtonToggled);

		if (Input.IsActionJustPressed("utility_used"))
			inputMap.Add(InputType.RightClickJustPressed);
		
		if (Input.IsActionPressed("default_attack")) 
			inputMap.Add(InputType.LeftClickHold);

		if (Input.IsActionJustReleased("default_attack"))
			inputMap.Add(InputType.LeftClickJustReleased);

		return inputMap;
	}

    public IPlayerAttackable FindInteractableWithinCursor() {
		List<Node2D> list = Utils.GetPreloadedScene<GlobalCursor>(hand, PreloadedScene.GlobalCursor).ObjectsInCursorRange;

        foreach(Node2D node2d in list) {
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
        //if nothing hit the ray, they we good.
        return result.Count <= 0;
    }
 
	readonly uint faceObjectMask = (uint) Layers.Environment + (uint) Layers.Enemies;
    private Actor FindObjectToFace(List<Actor> _) {
        //Check if the player is clicking/pressing on the screen. 
        foreach (Actor enemy in Player.Players[0].InteractableRadar.NearbyEnemies) {

            PhysicsDirectSpaceState2D spaceState = hand.GetWorld2D().DirectSpaceState;
            var ray = PhysicsRayQueryParameters2D.Create(hand.GlobalPosition, enemy.GlobalPosition, faceObjectMask);
            var result = spaceState.IntersectRay(ray);

            if (result.Count > 0 && (Rid) result["collider"] == enemy.GetRid())
                return enemy;
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

		if (hand.HeldWeapon is null) return;
		
		List<InputType> inputMap = GetAttackInputs();

		if (inputMap.Contains(InputType.LeftClickJustReleased)) OnWeaponLetGo?.Invoke();

		if (inputMap.Contains(InputType.RightClickJustPressed)) {
            targettedAttackable = FindInteractableWithinCursor();

            if (targettedAttackable is null)
				GUI.TargetIndicator.Disable();
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
				
				//If the interactable is still in tact and is still visible, autoshoot it.
				if (IsInteractableVisible(targettedAttackable)) {
					UpdateWeaponDirection?.Invoke(targettedAttackable.GetPosition());
					UseWeapon?.Invoke(delta);
				}
				break;

			case ControlMethod.Autoaim:
                Actor see = FindObjectToFace(Player.Players[0].InteractableRadar.NearbyEnemies);
				
				if (hand.HeldWeapon.WeaponType is Weapon.Type.HoldToCharge) {
					UseWeapon?.Invoke(delta); 
				}

				if (see is not null) {
					UpdateWeaponDirection?.Invoke(see.GlobalPosition);
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

public class DialogueController {
	readonly InputController inputController;
	readonly GUI GUI;
	public DialogueController(InputController inputController, GUI GUI) {
		this.inputController = inputController;
		this.GUI = GUI;
		
		// TODO: I don't like having to use this event :(
		GUI.DialogueBar.Ready += DialogueControlInit;
	}

	private bool WithinDialogueBar() {
		var rect = GUI.DialogueBar.GetGlobalRect();
		var mouse_position = GUI.DialogueBar.GetGlobalMousePosition();
		return rect.HasPoint(mouse_position);
	}
	private void DialogueControlInit() {
		GUI.DialoguePlayer.Started += (info) => inputController.UIInputFilter.SetFilterMode(info.PausePlayerInput);
		GUI.DialoguePlayer.Ended += () => inputController.UIInputFilter.SetFilterMode(false);
	}
	
	public void Continue() { if (Input.IsActionJustPressed("default_attack") && WithinDialogueBar()) GUI.DialoguePlayer.Clicked?.Invoke();}	
}

public class InteractablesButtonController {

	readonly Player player;
	public InteractablesButtonController(Player player, GUI GUI) {
		this.player = player;
		GUI.InteractButton.Pressed += InvokeInteracted;
	}

	public event Action<Player> Interacted;
	private void InvokeInteracted() {
		// this is a seperate method incase I wanna add some more logic to this sometime
		Interacted?.Invoke(player);
	}
}
