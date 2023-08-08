using Godot;
using System;
using System.Collections.Generic;
using KidoUtils;

using System.Threading.Tasks;
using System.Reflection.Metadata;

public class WeaponController {
	
	readonly WeaponManager hand;

	#region ATTACK INPUTS
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

		GUI.AttackButton.Pressed += () => isAutoAttackButtonToggled = !isAutoAttackButtonToggled;
    }

	private List<InputType> GetAttackInputs() {
		List<InputType> inputMap = new();

		if (isAutoAttackButtonToggled) {
			inputMap.Add(InputType.AutoAttackButtonToggled);
		}

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
        if (result.Count <= 0) {
            return true;
        }
        return false;
    }
 
	readonly uint faceObjectMask = (uint) Layers.Environment + (uint) Layers.Enemies;
    private Actor FindObjectToFace(List<Actor> _) {
        //Check if the player is clicking/pressing on the screen. 
        foreach (Actor enemy in Player.Players[0].NearbyEnemies) {

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
	
    public void UpdateWeapon(double delta) {

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

		if (useMethod is ControlMethod.SelectedAutoaim) {
			//If using a hold-to-charge weapon, the charge should increase when not looking at thing.
			if (hand.HeldWeapon.WeaponType is Weapon.Type.HoldToCharge) {
				UpdateWeaponDirection?.Invoke(targettedAttackable.GetPosition());
				UseWeapon?.Invoke(delta);
				return;
			}
			
			//If the interactable is still in tact and is still visible, autoshoot it.
			if (IsInteractableVisible(targettedAttackable)) {
				UpdateWeaponDirection?.Invoke(targettedAttackable.GetPosition());
				UseWeapon?.Invoke(delta);
			}
		}
		
		if (useMethod is ControlMethod.Autoaim) {
            Actor see = FindObjectToFace(Player.Players[0].NearbyEnemies);
            
			if (hand.HeldWeapon.WeaponType is Weapon.Type.HoldToCharge) {
				UseWeapon?.Invoke(delta); 
			}

            if (see is not null) {
				UpdateWeaponDirection?.Invoke(see.GlobalPosition);
				//This is literally the only solution i can  think of..
				if (hand.HeldWeapon.WeaponType is not Weapon.Type.HoldToCharge)
					UseWeapon?.Invoke(delta);
            }
        }
		
		if (useMethod is ControlMethod.ManualAim) {

			if (isHoveringOverGui) return;
			//If the player is aiming themselves, shoot where they're pointing.

			if (inputMap.Contains(InputType.LeftClickHold))
				UseWeapon?.Invoke(delta);
			
			//Default the weapon to point to the cursor.
			UpdateWeaponDirection?.Invoke(hand.GetGlobalMousePosition());
		}
	}

	#endregion 
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

	// TODO:
	// I don't like how the inputController has the
	// "FilterNonUIinput" field toggled from this class
	// It'll cause bugs I swear

	private void DialogueControlInit() {
		GUI.DialoguePlayer.DialogueStarted += (info) => inputController.FilterNonUiInput = info.PausePlayerInput;
		GUI.DialoguePlayer.DialogueEnded += () => inputController.FilterNonUiInput = false; 
	}
	
	public void ContinueDialogue() { if (Input.IsActionJustPressed("default_attack") && WithinDialogueBar()) GUI.DialoguePlayer.OnClicked?.Invoke();}	
}

public class InteractablesButtonController {

	public InteractablesButtonController(GUI GUI) {
		GUI.InteractButton.Pressed += InvokeInteracted;
	}

	public event Action Interacted;
	private void InvokeInteracted() {
		// this is a seperate method incase I wanna add some more logic to this sometime
		Interacted?.Invoke();
	}
}

public partial class InputController : Node {
	[Export]
	private Player attachedPlayer;
	[Export]
	private WeaponManager hand;

	bool filterNonUiInput = false;
	public bool FilterNonUiInput {
		get => filterNonUiInput;
		set {
			OnFilterModeChanged?.Invoke(value);
			filterNonUiInput = value;
		}
	}

	public event Action<bool> OnFilterModeChanged;
	
	private GUI GUI => attachedPlayer.GUI;
	
	// The (very tightly coupled) glue

	#region GUI
	public event Action LeftClicked; 

	private void InvokeLeftClickedWhenClickedSpecificallyForGUIPurposesOnly() {
		if (Input.IsActionJustPressed("default_attack")) LeftClicked?.Invoke();
	}
	#endregion
	
	public WeaponController WeaponController {get; private set;}
	[Export]
	public MovementController MovementController {get; private set;}
	public DialogueController DialogueController {get; private set;}
	// TODO: What a bad name
	public InteractablesButtonController InteractablesButtonController {get; private set;}

	public void Init(Player player) {
		attachedPlayer = player;
		
		WeaponController = new(hand, player);
		DialogueController = new(this, GUI);
		InteractablesButtonController = new(GUI);
		MovementController.Init(player, this);

		attachedPlayer.DamageableComponent.OnDeath += (_) => FilterNonUiInput = true;
	}
	public override void _Process(double delta) {
		// Allow player to interact with UI even if input is filtered.
		UpdateUIInput(delta);

		if (FilterNonUiInput) return;
		
		UpdateNonUIInput(delta);
	}

	private void UpdateUIInput(double _) {
		DialogueController.ContinueDialogue();
		InvokeLeftClickedWhenClickedSpecificallyForGUIPurposesOnly();
	}

	private void UpdateNonUIInput(double delta) {
		MovementController.ControlPlayerMovement();
		WeaponController.UpdateWeapon(delta);
	}

}