using System;
using Godot;


public interface IChestItem {
	public static PackedScene Temp {get; set;} 
}

public abstract partial class Weapon : Node2D {
	
	[Export]
	public Sprite2D Sprite {get; private set;}
	[Export]
	public bool UsesReloadVisuals {get; private set;}

	// Only get from the static PackedSceneResource.
	public abstract PackedScene PackedScene {get;}

	// Passes the added weapon
	public event Action<Weapon, Weapon> WeaponSwitched;
	// Passes the removed weapon

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
		//if (inputController is null) return;

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

	public void ChangeWeapon(Weapon newWeapon) {	
		InputController inputController = Hand.GetNode<InputController>("../Input Controller");

		inputController.UpdateWeaponDirection -= UpdateWeapon;
		inputController.UseWeapon -= OnWeaponUsing;
		inputController.OnWeaponLetGo -= OnWeaponLetGo;
		

		// Remove all of the weapon's nodes held by the hand

		foreach (Node child in Hand.GetChildren()) child.QueueFree();
		
		Weapon newWeaponInstance = newWeapon.PackedScene.Instantiate<Weapon>(); 

		// Add the new weapon
		Hand.AddChild(newWeaponInstance);

		WeaponSwitched?.Invoke(this, newWeaponInstance);

		// just making sur ethis is also removed.
		QueueFree();
	}

	//This type thing is relavent to the input controller.
	public enum Type {
		//1 click, 1 shot. Hold to constantly shoot.
		InstantShot,
		//Hold to shoot stronger
		HoldToCharge,
	}
}

