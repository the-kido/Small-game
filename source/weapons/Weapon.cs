using System;
using Godot;


public interface IChestItem {
	public static PackedScene Temp {get; set;} 
}

public abstract partial class Weapon : Node2D {

	[Export]
	public Sprite2D Sprite {get; private set;}

	public static PackedScene PackedSceneResource {get; protected set;}
	public PackedScene packedScene => PackedSceneResource;

	// Passes the added weapon
	public event Action<Weapon> WeaponAdded;
	// Passes the removed weapon
 	public event Action<Weapon> WeaponRemoved;

	public abstract Type WeaponType {get; protected set;} 
	public abstract void UpdateWeapon(Vector2 attackDirection);
	public virtual void OnWeaponLetGo() {}
	public abstract void Attack();
	public virtual void Init() {}


	[Export]
	public float ReloadSpeed {get; private set;} = 1;
	protected double reloadTimer = 0;

	protected Node2D Hand => GetParent<Node2D>();
	protected Player Player => Hand.GetParent<Player>();

    public override void _Ready() {
		Init();
		InputController inputController = Hand.GetNode<InputController>("../Input Controller");

		inputController.UpdateWeaponDirection += UpdateWeapon;
		inputController.UseWeapon += OnWeaponUsing;
		inputController.OnWeaponLetGo += OnWeaponLetGo;
	}


	//While the player is "using" (holding click for) the weapon.
	//Can be overridden if need be
	protected virtual void OnWeaponUsing(double delta) {
		reloadTimer += delta;

		if (reloadTimer < ReloadSpeed)
			return;
		
		reloadTimer = 0;
		Attack();
	}

	public void ChangeWeapon(Weapon weapon) {	

		InputController inputController = Hand.GetNode<InputController>("../Input Controller");

		inputController.UpdateWeaponDirection -= UpdateWeapon;
		inputController.UseWeapon -= OnWeaponUsing;
		inputController.OnWeaponLetGo -= OnWeaponLetGo;

		WeaponRemoved?.Invoke(this);

		// Remove all of the weapon's nodes held by the hand

		GD.Print(weapon.GetParent());

		foreach (Node child in Hand.GetChildren()) {
			child.QueueFree();
		}

		// Add the new weapon
		Hand.AddChild(weapon);
		WeaponAdded?.Invoke(weapon);

	}

	//This type thing is relavent to the input controller.
	public enum Type {
		//1 click, 1 shot. Hold to constantly shoot.
		InstantShot,
		//Hold to shoot stronger
		HoldToCharge,
	}
}

