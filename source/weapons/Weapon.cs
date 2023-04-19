using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;


public partial class Weapon : Node2D {
	[Export]
	public float ReloadSpeed {get; set;}
	protected static Node2D parentNode;


	public override void _Ready() {
		parentNode = (Node2D) GetParent();

		((PlayerInputController) parentNode.GetNode("../Input Controller")).UseWeapon += onPlayerWeaponUse;
	}

	public Weapon init(Node2D weaponHolder) {
		parentNode = weaponHolder;
		return this;
	}
	public virtual void useWeapon(string[] inputMap) {
	}

	public static void ChangeWeapon(PackedScene weapon) {
		foreach (Node child in parentNode.GetChildren()) {
			child.QueueFree();
		}
		Weapon newWeapon = ((Weapon) weapon.Instantiate());
		parentNode.AddChild(newWeapon);
	}

	private bool reloaded = true;

	private async void onPlayerWeaponUse(string[] inputMap) {
		if (!reloaded)
			//reload() is taking its time.
			return;
		reloaded = false;

		useWeapon(inputMap);
		GD.Print("PEW");
		await reload();


		reloaded = true;
	}

	private Task reload() {
		int delay = (int) (ReloadSpeed * 1000);
		return Task.Delay(delay);
	}
}

