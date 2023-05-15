using Godot;
using System;
using System.Collections.Generic;
using KidoUtils;

using System.Threading.Tasks;

public partial class InputController : Node
{
	[Export]
	private Player attachedPlayer;
	private PlayerHUD playerHUD {
		get {
			return attachedPlayer.HUD;
		}
	}

	[Export]
	private Node2D hand;
	public bool FilterAllInput {get; private set;} = false;
	
	#region ATTACK
	public event Action UseWeapon;
	public event Action<Vector2> UpdateWeapon;
	private bool isHoveringOverGui = false;
	private bool isAutoAttackButtonToggled = false;
	private List<InputType> GetAttackInputs() {
		List<InputType> inputMap = new();

		if (isAutoAttackButtonToggled) {
			inputMap.Add(InputType.AutoAttackButtonToggled);
		}
		if (Input.IsActionJustPressed("default_attack")) {
			inputMap.Add(InputType.LeftClicked);
		}
		if (Input.IsActionPressed("default_attack")) {
			inputMap.Add(InputType.LeftClickHold);
		}
		return inputMap;
	}
	private IInteractable targettedObject;
    public IInteractable FindInteractable(List<Node2D> list) {
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

 	private void ControlWeapon() {
		List<InputType> inputMap = GetAttackInputs();


		//If the player left clicks, find the nearest IInteractable to focus on.
		if (inputMap.Contains(InputType.LeftClicked)) {
			GlobalCursor cursor = KidoUtils.Utils.GetPreloadedScene<GlobalCursor>(this, PreloadedScene.GlobalCursor);
            targettedObject = FindInteractable(cursor.ObjectsInCursorRange);
			return;
        }


		if(targettedObject?.IsInteractable() == true) {

			//Manipultate the target indicator to show which enemy you're autoshooting
			playerHUD.TargetIndicator.Enable(true, targettedObject);

			//If the interactable is still in tact and is still visible, autoshoot it.
			if(IsInteractableVisible(targettedObject)) {
				UpdateWeapon?.Invoke(targettedObject.GetPosition());
				UseWeapon?.Invoke();
				return;
			}
		}
		
		if (inputMap.Contains(InputType.AutoAttackButtonToggled)) {

            Actor see = FindObjectToFace(Player.players[0].NearbyEnemies);
            
            if (see is not null) {
				UpdateWeapon?.Invoke(see.GlobalPosition);
				UseWeapon?.Invoke();
            }
            return;
        }
		//
		if(isHoveringOverGui) return;

		//If the player is aiming themselves, shoot where they're pointing. 
		if (inputMap.Contains(InputType.LeftClickHold)) {
			UseWeapon?.Invoke();
		}

		//Default the weapon to point to the cursor.
		UpdateWeapon?.Invoke(hand.GetGlobalMousePosition());
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
		playerHUD.AttackButton.OnAttackButtonPressed += () => isAutoAttackButtonToggled = !isAutoAttackButtonToggled;
		playerHUD.AttackButton.OnMouseEntered += () => isHoveringOverGui = true;
		playerHUD.AttackButton.OnMouseExited += () => isHoveringOverGui = false;

		attachedPlayer.DamageableComponent.OnDeath += OnDeath;
	}
	public override void _Process(double delta)
	{
		if (FilterAllInput)
			return;
		
		ControlWeapon();
		ControlMovement();
	}

	private void OnDeath (DamageInstance _) {
		FilterAllInput = true;
	}
}

public enum InputType {
	LeftClicked,
	LeftClickHold,
	RightClickHold,
	AutoAttackButtonToggled,
}