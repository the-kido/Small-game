using Godot;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


public abstract partial class Weapon : Node2D {


	//This type thing is relavent to the input controller.
	public enum Type {
		//1 click, 1 shot. Hold to constantly shoot.
		InstantShot,
		//Hold to shoot stronger
		HoldToCharge,
		//
	}
	public abstract Weapon.Type WeaponType {get; protected set;} 

	[Export]
	public float ReloadSpeed {get; private set;}
	protected double reloadTimer = 0;

	protected Node2D hand => (Node2D) GetParent();
	public override void _Ready() {
		Name = "Weapon";

		InputController inputController = hand.GetNode<InputController>("../Input Controller");

		inputController.UpdateWeaponDirection += UpdateWeapon;
		inputController.UseWeapon += OnWeaponUsing;
		inputController.OnWeaponLetGo += () => OnWeaponLetGo();
	}

	public abstract void UpdateWeapon(Vector2 attackDirection);
	public abstract void OnWeaponLetGo();
	public abstract void Attack();


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

		foreach (Node child in hand.GetChildren()) child.QueueFree();

		hand.AddChild(weapon.Instantiate());
	}
}

