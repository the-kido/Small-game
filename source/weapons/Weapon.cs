using Godot;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


public abstract partial class Weapon : Node2D {
	[Export]
	public float ReloadSpeed {get; set;}
	protected static Node2D hand;

	public override void _Ready() {
		hand = (Node2D) GetParent();
		Name = "Weapon";
		hand.GetNode<InputController>("../Input Controller").UpdateWeapon += UpdateWeapon;
	}

	///<Summary>
	///Can be overriden to implement functionality when the weapon is eventually used.
	///</Summary>
	public abstract void UpdateWeapon(List<InputType> inputMap);
	public abstract void UseWeapon();
	private bool reloaded = true;
	public async void AttackAndReload(bool immediate) {
		if (!reloaded)
			return;
		
		reloaded = false;

		if (immediate) {
			UseWeapon();
			await Task.Delay((int)(ReloadSpeed * 1000));
		}else{
			await Task.Delay((int)(ReloadSpeed * 1000));
			UseWeapon();
		}

		reloaded = true;
	}

	public void ChangeWeapon(PackedScene weapon) {
		hand.GetNode<InputController>("../Input Controller").UpdateWeapon -= UpdateWeapon;

		foreach (Node child in hand.GetChildren()) child.QueueFree();

		hand.AddChild(weapon.Instantiate());
	}
}

