using Godot;
using Game.Players;
using Game.Players.Mechanics;
using Game.Players.Inputs;
using Game.Actors;
using System.Reflection.Metadata;

public enum ChestItemType {
	WEAPON,
	SHIELD
}

public interface IChestItem {
	public abstract string Description {get;}
	public abstract Texture2D Icon {get;}
	public abstract ChestItemType Type {get;} 
}

public abstract partial class Weapon : Node2D, IChestItem {
    public Texture2D Icon {get => sprite.Texture;}
    public abstract string Description {get;}
	ChestItemType IChestItem.Type {get => ChestItemType.WEAPON; }
	[Export]
	private Sprite2D sprite;
	
	[Export]
	public bool UsesReloadVisuals {get; private set;}

	// Only get from the static PackedSceneResource.
	public abstract PackedScene PackedScene {get;}
	public abstract Type WeaponType {get; protected set;} 
	public abstract void UpdateWeapon(Vector2 attackDirection);
	public abstract void Attack();
	// These don't have to be implemented so keep them virtual
	public virtual void OnWeaponLetGo() {}
	public virtual void Init() {}

	[Export]
	public float BaseReloadSpeed {get; private set;} = 1;
	public float EffectiveReloadSpeed => Hand.reloadSpeed.GetEffectiveValue(BaseReloadSpeed);

	protected double reloadTimer = 0;

	protected WeaponManager Hand => GetParent<WeaponManager>();
	protected Player Player => Hand?.GetParent<Player>();

    public override void _Ready() => Init();

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

		if (reloadTimer < EffectiveReloadSpeed)
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

