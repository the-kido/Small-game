using Godot;
using Game.Players;
using Game.Players.Mechanics;

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

	[Export]
	public float BaseReloadSpeed {get; private set;} = 1;
	public float EffectiveReloadSpeed => Hand.reloadSpeed.GetEffectiveValue(BaseReloadSpeed);

	protected double reloadTimer = 0;

	protected WeaponManager Hand {get; private set;}
	protected Player Player => Hand.GetParent<Player>();

	public void Init(WeaponManager hand, int slot) {
		Name = $"{Name}: {slot}";
		Hand = hand;
		// this.ToggleYSorting();
		Visible = false;
	}

	public void AttachEvents() {
		Hand.WeaponController.UpdateWeaponDirection += UpdateWeapon;
		Hand.WeaponController.UseWeapon += OnWeaponUsing;
		Hand.WeaponController.OnWeaponLetGo += OnWeaponLetGo;
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

	public void DoSafetyChecks() {
		KidoUtils.ErrorUtils.AvoidImproperOrdering(this);
	}
}

