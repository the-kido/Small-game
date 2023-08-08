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

	// Passes the removed weapon
	public abstract Type WeaponType {get; protected set;} 
	public abstract void UpdateWeapon(Vector2 attackDirection);
	public virtual void OnWeaponLetGo() {}
	public abstract void Attack();
	public virtual void Init() {}

	[Export]
	public float ReloadSpeed {get; private set;} = 1;
	protected double reloadTimer = 0;

	protected WeaponManager Hand => GetParent<WeaponManager>();
	protected Player Player => Hand.GetParent<Player>();

    public override void _Ready() {
		Init();
	}

	public void Enable(bool enable) {
        Visible = enable;
        
		if (enable)
			AttachEvents();
        else
            DetachEvents();
	}

	private void AttachEvents() {
		WeaponController weaponController = Hand.GetNode<InputController>("../Input Controller").WeaponController;

		weaponController.UpdateWeaponDirection += UpdateWeapon;
		weaponController.UseWeapon += OnWeaponUsing;
		weaponController.OnWeaponLetGo += OnWeaponLetGo;
	}
	private void DetachEvents() {
		WeaponController weaponController = Hand.GetNode<InputController>("../Input Controller").WeaponController;

		weaponController.UpdateWeaponDirection -= UpdateWeapon;
		weaponController.UseWeapon -= OnWeaponUsing;
		weaponController.OnWeaponLetGo -= OnWeaponLetGo;
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

	//This type thing is relavent to the input controller.
	public enum Type {
		//1 click, 1 shot. Hold to constantly shoot.
		InstantShot,
		//Hold to shoot stronger
		HoldToCharge,
	}
}

