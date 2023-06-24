using Godot;
using System;
using System.Collections.Generic;
using KidoUtils;

using System.Threading.Tasks;

public partial class InputController : Node
{
	[Export]
	private Player attachedPlayer;
	[Export]
	private Node2D hand;

	bool filterNonUiInput = false;
	public bool FilterNonUiInput {
		get {
			return filterNonUiInput;
		}
		set {
			OnFilterModeChanged?.Invoke(value);
			filterNonUiInput = value;
		}
	}
	public event Action<bool> OnFilterModeChanged;
	
	private GUI GUI => attachedPlayer.GUI;
	

	#region DIALOGUE
	private bool WithinDialogueBar() {
		var rect = GUI.DialogueBar.GetGlobalRect();
		var mouse_position = GUI.DialogueBar.GetGlobalMousePosition();
		return rect.HasPoint(mouse_position);
	}
	private void DialogueControlInit() {
		GUI.DialoguePlayer.DialogueStarted += (info) => FilterNonUiInput = info.pausePlayerInput;
		GUI.DialoguePlayer.DialogueEnded += () => FilterNonUiInput = false; 
	}
	
	private void ContinueDialogue() { if (Input.IsActionJustPressed("default_attack") && WithinDialogueBar()) GUI.DialoguePlayer.OnClicked?.Invoke();}	

	#endregion

	#region ATTACK
	public event Action<double> UseWeapon;
	public event Action<Vector2> UpdateWeaponDirection;
	public event Action OnWeaponLetGo;
	
	private bool isHoveringOverGui = false;
	private bool isAutoAttackButtonToggled = false;
	
	private Weapon weapon => hand.GetChild<Weapon>(0);
	
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
		List<Node2D> list = KidoUtils.Utils.GetPreloadedScene<GlobalCursor>(this, PreloadedScene.GlobalCursor).ObjectsInCursorRange;

        foreach(Node2D node2d in list) {
            if (node2d is IPlayerAttackable) {
                return (IPlayerAttackable) node2d;
            }
        }
        return null;
    }

    private bool IsInteractableVisible(IPlayerAttackable interactable) {
        PhysicsDirectSpaceState2D spaceState = hand.GetWorld2D().DirectSpaceState;
        var ray = PhysicsRayQueryParameters2D.Create(hand.GlobalPosition, interactable.GetPosition(), (uint) Layers.Enviornment);
        var result = spaceState.IntersectRay(ray);
        //if nothing hit the ray, they we good.
        if (result.Count <= 0) {
            return true;
        }
        return false;
    }
 
	uint faceObjectMask = (uint) Layers.Enviornment + (uint) Layers.Enemies;
    private Actor FindObjectToFace(List<Actor> enemies) {
        //Check if the player is clicking/pressing on the screen. 
        foreach (Actor enemy in Player.players[0].NearbyEnemies) {

            PhysicsDirectSpaceState2D spaceState = hand.GetWorld2D().DirectSpaceState;
            var ray = PhysicsRayQueryParameters2D.Create(hand.GlobalPosition, enemy.GlobalPosition, faceObjectMask);
            var result = spaceState.IntersectRay(ray);

            if (result.Count > 0 && (Rid) result["collider"] == enemy.GetRid())
                return enemy;
        }
        return null;
    }

	enum WeaponControl { Autoaim, SelectedAutoaim, ManualAim }
	private WeaponControl useMethod; 

	private WeaponControl GetUseMethod(IPlayerAttackable targettedInteractable) {
		List<InputType> inputMap = GetAttackInputs();

		// If something happened to the interactable, default to default aim method.
		if (useMethod is WeaponControl.SelectedAutoaim && targettedInteractable is null) return WeaponControl.ManualAim;
		

		if (inputMap.Contains(InputType.AutoAttackButtonToggled)) return WeaponControl.Autoaim;

		// This is put last for most priority. If the player explicitly wants to target a unit, it should 
		
		if (targettedInteractable is not null && targettedInteractable.IsInteractable()) return WeaponControl.SelectedAutoaim;
		
		// Default to manual aim
		return WeaponControl.ManualAim;
	}
	
	IPlayerAttackable targettedAttackable = null;
	private void UpdateWeapon(double delta) {
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

		if (useMethod is WeaponControl.SelectedAutoaim) {
			//If using a hold-to-charge weapon, the charge should increase when not looking at thing.
			if (weapon.WeaponType is Weapon.Type.HoldToCharge) {
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
		
		if (useMethod is WeaponControl.Autoaim) {
            Actor see = FindObjectToFace(Player.players[0].NearbyEnemies);
            
			if (weapon.WeaponType is Weapon.Type.HoldToCharge) {
				UseWeapon?.Invoke(delta); 
			}

            if (see is not null) {
				UpdateWeaponDirection?.Invoke(see.GlobalPosition);
				//This is literally the only solution i can  think of..
				if (weapon.WeaponType is not Weapon.Type.HoldToCharge)
					UseWeapon?.Invoke(delta);
            }
        }
		
		if (useMethod is WeaponControl.ManualAim ) {
			//If the player is aiming themselves, shoot where they're pointing. 
			if (inputMap.Contains(InputType.LeftClickHold))
				UseWeapon?.Invoke(delta);
			
			//Default the weapon to point to the cursor.
			UpdateWeaponDirection?.Invoke(hand.GetGlobalMousePosition());
		}
	}

	#endregion 

	#region MOVEMENT
	public event Action<Vector2> UpdateMovement;
	private void GetMovementInput() {
		Vector2 direction = new Vector2(
			Input.GetAxis("left", "right"),
			Input.GetAxis("up", "down")
		).Normalized();

		UpdateMovement?.Invoke(direction);
	}

	#endregion
	
	// The (very tightly coupled) glue

	public override void _Ready() {
		GUI.AttackButton.OnAttackButtonPressed += () => isAutoAttackButtonToggled = !isAutoAttackButtonToggled;
		GUI.AttackButton.OnMouseEntered += () => isHoveringOverGui = true;
		GUI.AttackButton.OnMouseExited += () => isHoveringOverGui = false;

		attachedPlayer.DamageableComponent.OnDeath += OnDeath;
		
		GUI.DialogueBar.Ready += DialogueControlInit;
	}
	public override void _Process(double delta) {
		// Allow player to interact with UI even if input is filtered.
		ContinueDialogue();
		
		if (FilterNonUiInput) {
			// How do I avoid this?
			attachedPlayer.Velocity = Vector2.Zero;
			return;
		}
		GetMovementInput();
		UpdateWeapon(delta);
	}
	private void OnDeath(DamageInstance _) {
		FilterNonUiInput = true;
		attachedPlayer.Velocity = Vector2.Zero;
	}
}
