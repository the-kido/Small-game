using Godot;
using System;
using System.Collections.Generic;
using KidoUtils;

using System.Threading.Tasks;

public partial class InputController : Node
{
	[Export]
	private Player attachedPlayer;
	private GUI GUI  => attachedPlayer.GUI;

	[Export]
	private Node2D hand;
	private Weapon weapon => hand.GetChild<Weapon>(0);
	public bool FilterAllInput {get; set;} = false;
	
	#region DIALOGUE
	private bool WithinDialogueBar() {
		var rect = GUI.HUD.dialogueBar.GetGlobalRect();
		var mouse_position = GUI.HUD.dialogueBar.GetGlobalMousePosition();

		return rect.HasPoint(mouse_position);
	}
	private DialoguePlayer dialoguePlayer => GUI.HUD.dialogueBar.dialoguePlayer;
	private void DialogueControlInit() {


	}
	private bool ClickedOnDialogueBar() => (Input.IsActionJustPressed("default_attack") && WithinDialogueBar()) ? true : false; 
	
	private void ContinueDialogue() {
		if (ClickedOnDialogueBar()) {
			dialoguePlayer.OnClicked?.Invoke();
		}
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
    public IInteractable FindInteractableWithinCursor() {
		List<Node2D> list = KidoUtils.Utils.GetPreloadedScene<GlobalCursor>(this, PreloadedScene.GlobalCursor).ObjectsInCursorRange;

        foreach(Node2D node2d in list) {
            if (node2d is IInteractable) {
                return (IInteractable) node2d;
            }
        }
        return null;
    }

    private bool IsInteractableVisible(IInteractable interactable) {
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
    private bool waiting = false;
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
	//This method does WAYY too much. Should be split up to avoid ultra confusion
	private IInteractable targettedInteractable;

	private void ControlUseMethod() {
		List<InputType> inputMap = GetAttackInputs();

		// Check if there is an interactable selected after right clicking.
		if (inputMap.Contains(InputType.RightClickJustPressed)) targettedInteractable = FindInteractableWithinCursor();

		// If something happened to the interactable, default to default aim method.
		if (useMethod is WeaponControl.SelectedAutoaim && targettedInteractable is null) useMethod = WeaponControl.ManualAim;
		
		if (inputMap.Contains(InputType.AutoAttackButtonToggled))
			useMethod = WeaponControl.Autoaim;
		else
			useMethod = WeaponControl.ManualAim;

		// This is put last for most priority. If the player explicitly wants to target a unit, it should 
		// Override any automation
		if (targettedInteractable is not null) useMethod = WeaponControl.SelectedAutoaim;

	}
 
	private void ControlWeapon(double delta) {

		List<InputType> inputMap = GetAttackInputs();

		if (inputMap.Contains(InputType.LeftClickJustReleased)) OnWeaponLetGo?.Invoke();

		if (inputMap.Contains(InputType.RightClickJustPressed)) {
            targettedInteractable = FindInteractableWithinCursor();

			if (targettedInteractable is null)
				GUI.TargetIndicator.Disable();
			else
				GUI.TargetIndicator.Enable(targettedInteractable);
		}

		if (useMethod is WeaponControl.SelectedAutoaim && targettedInteractable?.IsInteractable() == true) {
			//If using a hold-to-charge weapon, the charge should increase when not looking at thing.
			if (weapon.WeaponType is Weapon.Type.HoldToCharge) {
				UpdateWeaponDirection?.Invoke(targettedInteractable.GetPosition());
				UseWeapon?.Invoke(delta);
				return;
			}
			
			//If the interactable is still in tact and is still visible, autoshoot it.
			if (IsInteractableVisible(targettedInteractable)) {
				UpdateWeaponDirection?.Invoke(targettedInteractable.GetPosition());
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
	private void ControlMovement() {
		Vector2 direction = new Vector2(
			Input.GetAxis("left", "right"),
			Input.GetAxis("up", "down")
		).Normalized();

		UpdateMovement?.Invoke(direction);
	}

	#endregion
	
	public override void _Ready() {
		GUI.HUD.AttackButton.OnAttackButtonPressed += () => isAutoAttackButtonToggled = !isAutoAttackButtonToggled;
		GUI.HUD.AttackButton.OnMouseEntered += () => isHoveringOverGui = true;
		GUI.HUD.AttackButton.OnMouseExited += () => isHoveringOverGui = false;

		attachedPlayer.DamageableComponent.OnDeath += OnDeath;
		
		GUI.HUD.dialogueBar.Ready += DialogueControlInit;
	}
	public override void _Process(double delta) {
		if (FilterAllInput)
			return;
		ControlMovement();
		ControlUseMethod();
		ControlWeapon(delta);

		ContinueDialogue();

		// It's only fair the next method starts with a C as well
	}
	private void OnDeath(DamageInstance _) {
		FilterAllInput = true;
		attachedPlayer.Velocity = Vector2.Zero;
	}
}

public enum InputType {
	RightClickJustPressed,
	LeftClickHold,
	LeftClickJustReleased,
	RightClickHold,
	AutoAttackButtonToggled,
}