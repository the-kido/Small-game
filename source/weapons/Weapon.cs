using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;


public partial class Weapon : Node2D
{
	[Export]
	public float ReloadSpeed {get; set;}
	private AnimationPlayer animationPlayer;
	private Node2D parentNode;
	public Weapon SelectedWeapon {get; private set;}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = (AnimationPlayer) GetNode("AnimationTree");
		parentNode = (Node2D) GetParent();
	}

	public Weapon init(Node2D weaponHolder) {
		parentNode = weaponHolder;
		return this;
	}
	public void useWeapon(string[] inputMap) {
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		FaceWeaponToCursor();
	}
	private void FaceWeaponToCursor() {
		parentNode.LookAt(GetGlobalMousePosition());
	}


	public void ChangeWeapon(PackedScene weapon) {
		foreach (Node child in GetChildren()) {
			child.QueueFree();
		}
		Weapon newWeapon = ((Weapon) weapon.Instantiate()).init(this);
		AddChild(newWeapon);

		SelectedWeapon = newWeapon;
		ReloadSpeed = newWeapon.ReloadSpeed;
	}

	private bool reloaded = true;
	private async void onPlayerWeaponUse(string[] inputMap) {
		if (!reloaded)
			//reload() is taking its time.
			return;
		reloaded = false;
		SelectedWeapon.useWeapon(inputMap);
		GD.Print("waiting...");

		await reload();
		
		GD.Print("Done waiting!");

		reloaded = true;
	}

	private Task reload() {

		//double reloadDelayRecharge = 0f;
		int delay = (int) ReloadSpeed*1000;
		return Task.Delay(delay);


		// while (reloadDelayRecharge < reloadDelay) {
		// 	int delay = (int) (GetProcessDeltaTime() * 1000f);
		// 	await Task.Delay(delay);
		// 	reloadDelayRecharge += GetProcessDeltaTime();
		// }
		// reloadDelayRecharge = 0;
		// return ;
	}
}

