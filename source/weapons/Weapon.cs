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
		InputController inputController = hand.GetNode<InputController>("../Input Controller");

		inputController.UpdateWeapon += UpdateWeapon;
		inputController.UseWeapon += UseAndReloadWeapon;
	}

	public abstract void UpdateWeapon(Vector2 attackDirection);
	public abstract void Attack();

	private bool reloaded = true;
	private async void UseAndReloadWeapon() {
		if (!reloaded)
			return;
		
		reloaded = false;

		Attack();
		await Task.Delay((int)(ReloadSpeed * 1000));

		reloaded = true;
	}

	public void ChangeWeapon(PackedScene weapon) {
		InputController inputController = hand.GetNode<InputController>("../Input Controller");

		inputController.UpdateWeapon -= UpdateWeapon;
		inputController.UseWeapon -= UseAndReloadWeapon;

		foreach (Node child in hand.GetChildren()) child.QueueFree();

		hand.AddChild(weapon.Instantiate());
	}
}

