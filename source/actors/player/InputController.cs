using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class InputController : Node
{
	[Signal]
	public delegate void UseWeaponEventHandler(string[] inputMap);
	public bool FilterAllInput {set; get;} = false;


	#region All attack related input methods 
	private void DetectAttactInput() {
		var inputMap = new List<string>();
		if (Input.IsActionPressed("default_attack")) {
			inputMap.Add("Left Click");
		}
		if (inputMap.Count > 0) {
			OnAttackKeyHeld(inputMap.ToArray());
		}
	}
	private bool reloaded = true;
	private async void OnAttackKeyHeld(string[] inputMap) {
		if (!reloaded)
			return;
		
		reloaded = false;
		
		EmitSignal(SignalName.UseWeapon, inputMap);

		await Task.Delay((int)(GetNode("../Hand").GetNode<Weapon>("Weapon").ReloadSpeed * 1000));

		reloaded = true;
	}

	#endregion 

	public override void _Process(double delta)
	{
		if (FilterAllInput)
			return;
		
		DetectAttactInput();
	}
}
