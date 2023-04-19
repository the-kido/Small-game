using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;


public partial class Weapon : Node2D
{
	[Export]
	public float ReloadSpeed {get; set;}
	private AnimationTree animationPlayer;
	private static Node2D parentNode;
	//public Weapon SelectedWeapon {get; private set;}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("ready!");
		animationPlayer = (AnimationTree) GetNode("AnimationTree");
		parentNode = (Node2D) GetParent();

		((PlayerInputController) parentNode.GetNode("../Input Controller")).UseWeapon += onPlayerWeaponUse;
	}
	
	#region Put this in a superset weapon, maybe "Gun" 
	// public override void _Process(double delta)
	// {
	// 	FaceWeaponToCursor();
	// }
	// public virtual void FaceWeaponToCursor() {
	// 	parentNode.LookAt(GetGlobalMousePosition());
	// }
	#endregion

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
		await reload();

		reloaded = true;
	}

	private Task reload() {
		int delay = (int) (ReloadSpeed * 1000);
		return Task.Delay(delay);
	}
}

