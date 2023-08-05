using Godot;
using System;
using System.Collections.Generic;
using KidoUtils;

using System.Threading.Tasks;
using System.Reflection.Metadata;

public partial class InputController : Node
{
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
	
	#region DIALOGUE
	private bool WithinDialogueBar() {
		var rect = GUI.DialogueBar.GetGlobalRect();
		var mouse_position = GUI.DialogueBar.GetGlobalMousePosition();
		return rect.HasPoint(mouse_position);
	}
	private void DialogueControlInit() {
		GUI.DialoguePlayer.DialogueStarted += (info) => FilterNonUiInput = info.PausePlayerInput;
		GUI.DialoguePlayer.DialogueEnded += () => FilterNonUiInput = false; 
	}
	
	private void ContinueDialogue() { if (Input.IsActionJustPressed("default_attack") && WithinDialogueBar()) GUI.DialoguePlayer.OnClicked?.Invoke();}	

	#endregion

	#region ATTACK INPUTS
	public event Action<double> UseWeapon;
	public event Action<Vector2> UpdateWeaponDirection;
	public event Action OnWeaponLetGo;
	
	private bool isHoveringOverGui = false;
	private bool isAutoAttackButtonToggled = false;
	
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
		List<Node2D> list = Utils.GetPreloadedScene<GlobalCursor>(this, PreloadedScene.GlobalCursor).ObjectsInCursorRange;

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
 
	uint faceObjectMask = (uint) Layers.Environment + (uint) Layers.Enemies;
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

		if (hand.GetChildren().Count is 0) return;
		
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
			if (hand.heldWeapon.WeaponType is Weapon.Type.HoldToCharge) {
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
            Actor see = FindObjectToFace(Player.Players[0].NearbyEnemies);
            
			if (hand.heldWeapon.WeaponType is Weapon.Type.HoldToCharge) {
				UseWeapon?.Invoke(delta); 
			}

            if (see is not null) {
				UpdateWeaponDirection?.Invoke(see.GlobalPosition);
				//This is literally the only solution i can  think of..
				if (hand.heldWeapon.WeaponType is not Weapon.Type.HoldToCharge)
					UseWeapon?.Invoke(delta);
            }
        }
		
		if (useMethod is WeaponControl.ManualAim) {

			if (isHoveringOverGui) return;
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
	
	#region INTERACTABLES

	public event Action Interacted;
	private void InvokeInteracted() {
		// this is a seperate method incase I wanna add some more logic to this sometime
		Interacted?.Invoke();
	}

	private void InitInteractableButton() {
		GUI.InteractButton.Pressed += InvokeInteracted;
	}

	#endregion
	// The (very tightly coupled) glue

	#region GUI
	public event Action LeftClicked; 

	private void InvokeLeftClickedWhenClickedSpecificallyForGUIPurposesOnly() {
		if (Input.IsActionJustPressed("default_attack")) LeftClicked?.Invoke();
	}
	#endregion
	public void Init(Player player) {
		attachedPlayer = player;
		attachedPlayer.InputController = this;

		GUI.AttackButton.Pressed += () => isAutoAttackButtonToggled = !isAutoAttackButtonToggled;

		attachedPlayer.DamageableComponent.OnDeath += OnDeath;
		
		GUI.DialogueBar.Ready += DialogueControlInit;

		InitInteractableButton();
	}
	public override void _Process(double delta) {
		// Allow player to interact with UI even if input is filtered.
		ContinueDialogue();
		InvokeLeftClickedWhenClickedSpecificallyForGUIPurposesOnly();
		
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



/*
using Godot;
using System;
using System.Collections.Generic;
using KidoUtils;




internal class WeaponControl {
	private readonly Player player;
	private readonly Node2D hand;
	private GUI GUI => player.GUI;
	private Weapon Weapon => hand.GetChild<Weapon>(0);

	public WeaponControl(Player player, Node2D hand) {
		this.player = player;
		this.hand = hand;
		GUI.AttackButton.Pressed += () => isAutoAttackButtonToggled = !isAutoAttackButtonToggled;
	}

	#region HELD WEAPON
	
	public void SetHeldWeapon(int slot) {
		Weapon.ChangeWeapon(player.GetWeapon(slot));
	}

	#endregion

	#region ATTACK
	public event Action<double> UseWeapon;
	public event Action<Vector2> UpdateWeaponDirection;
	public event Action OnWeaponLetGo;
	
	private bool isHoveringOverGui = false;
	private bool isAutoAttackButtonToggled = false;
	
	
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
	
	IPlayerAttackable targettedAttackable = null;
	public void UpdateWeapon(double delta) {

		if (hand.GetChildren().Count is 0) return;
		
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
			if (Weapon.WeaponType is Weapon.Type.HoldToCharge) {
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
            
			if (Weapon.WeaponType is Weapon.Type.HoldToCharge) {
				UseWeapon?.Invoke(delta); 
			}

            if (see is not null) {
				UpdateWeaponDirection?.Invoke(see.GlobalPosition);
				//This is literally the only solution i can  think of..
				if (Weapon.WeaponType is not Weapon.Type.HoldToCharge)
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

public partial class InputController : Node
{
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
		GUI.DialoguePlayer.DialogueStarted += (info) => FilterNonUiInput = info.PausePlayerInput;
		GUI.DialoguePlayer.DialogueEnded += () => FilterNonUiInput = false; 
	}
	
	private void ContinueDialogue() { if (Input.IsActionJustPressed("default_attack") && WithinDialogueBar()) GUI.DialoguePlayer.OnClicked?.Invoke();}	

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
	
	#region INTERACTABLES

	public event Action Interacted;
	private void InvokeInteracted() {
		// this is a seperate method incase I wanna add some more logic to this sometime
		Interacted?.Invoke();
	}

	private void InitInteractableButton() {
		GUI.InteractButton.Pressed += InvokeInteracted;
	}

	#endregion
	// The (very tightly coupled) glue

	#region GUI
	public event Action LeftClicked; 

	private void InvokeLeftClickedWhenClickedSpecificallyForGUIPurposesOnly() {
		if (Input.IsActionJustPressed("default_attack")) LeftClicked?.Invoke();
	}
	#endregion

	WeaponControl weaponControl;

	public void Init(Player player) {
		weaponControl = new(player, hand);

		attachedPlayer = player;

		attachedPlayer.InputController = this;


		attachedPlayer.DamageableComponent.OnDeath += OnDeath;
		
		GUI.DialogueBar.Ready += DialogueControlInit;

		InitInteractableButton();
	}
	public override void _Process(double delta) {
		// Allow player to interact with UI even if input is filtered.
		ContinueDialogue();
		InvokeLeftClickedWhenClickedSpecificallyForGUIPurposesOnly();
		
		if (FilterNonUiInput) {
			// How do I avoid this?
			attachedPlayer.Velocity = Vector2.Zero;
			return;
		}
		GetMovementInput();
		weaponControl.UpdateWeapon(delta);

	}
	private void OnDeath(DamageInstance _) {
		FilterNonUiInput = true;
		attachedPlayer.Velocity = Vector2.Zero;
	}
}
*/