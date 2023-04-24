using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;


public abstract partial class Weapon : Node2D {
	[Export]
	public float ReloadSpeed {get; set;}
	protected static Node2D hand;

	public override void _Ready() {
		hand = (Node2D) GetParent();
		Name = "Weapon";
		hand.GetNode<InputController>("../Input Controller").UseWeapon += useWeapon;
	}

	///<Summary>
	///Can be overriden to implement functionality when the weapon is eventually used.
	///</Summary>
	public abstract void useWeapon(string[] inputMap);

	public void ChangeWeapon(PackedScene weapon) {
		hand.GetNode<InputController>("../Input Controller").UseWeapon -= useWeapon;

		foreach (Node child in hand.GetChildren()) {

			child.QueueFree();
		}
		Weapon newWeapon = ((Weapon) weapon.Instantiate());
		hand.AddChild(newWeapon);
	}
}

