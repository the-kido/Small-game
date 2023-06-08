using System;
using Godot;

public abstract partial class Weapon : Node2D {

	// Passes the added weapon
	public event Action<Weapon> WeaponAdded;
	// Passes the removed weapon
 	public event Action<Weapon> WeaponRemoved;

	public abstract Weapon.Type WeaponType {get; protected set;} 
	public abstract void UpdateWeapon(Vector2 attackDirection);
	public virtual void OnWeaponLetGo() {}
	public abstract void Attack();
	public virtual void Init() {}


	[Export]
	public float ReloadSpeed {get; private set;} = 1;
	protected double reloadTimer = 0;

	protected Node2D hand => (Node2D) GetParent();
	public override void _Ready() {
		Init();
		InputController inputController = hand.GetNode<InputController>("../Input Controller");

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

	public void ChangeWeapon(PackedScene weapon) {	

		InputController inputController = hand.GetNode<InputController>("../Input Controller");

		inputController.UpdateWeaponDirection -= UpdateWeapon;
		inputController.UseWeapon -= OnWeaponUsing;
		inputController.OnWeaponLetGo -= OnWeaponLetGo;

		WeaponRemoved?.Invoke(this);

		// Remove all of the weapon's nodes held by the hand
		foreach (Node child in hand.GetChildren()) 
			child.QueueFree();

		// Add the new weapon
		Weapon newWeapon = weapon.Instantiate<Weapon>(); 
		hand.AddChild(newWeapon);
		WeaponAdded?.Invoke(newWeapon);

	}

	//This type thing is relavent to the input controller.
	public enum Type {
		//1 click, 1 shot. Hold to constantly shoot.
		InstantShot,
		//Hold to shoot stronger
		HoldToCharge,
	}
}

