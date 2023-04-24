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
		_init(hand);
		
		((PlayerInputController) hand.GetNode("../Input Controller")).UseWeapon += useWeapon;
	}
	
	public Weapon _init(Node2D weaponHolder) {
		Name = "Weapon";
		hand = weaponHolder;
		return this;
	}
	///<Summary>
	///Can be overriden to implement functionality when the weapon is eventually used.
	///</Summary>
	public abstract void useWeapon(string[] inputMap);

	public static void ChangeWeapon(PackedScene weapon) {
		foreach (Node child in hand.GetChildren()) {
			child.QueueFree();
		}
		Weapon newWeapon = ((Weapon) weapon.Instantiate());
		hand.AddChild(newWeapon);
	}
}

