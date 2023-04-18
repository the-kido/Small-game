using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInputController : Node
{
	[Signal]
	public delegate void UseWeaponEventHandler(string[] inputMap);
	public bool FilterAllInput {set; get;} = false;

	private void DetectAttactInput() {
		var inputMap = new List<string>();
		if (Input.IsActionPressed("default_attack")) {
			inputMap.Add("Left Click");
		}
		if (inputMap.Count > 0) {
			EmitSignal(SignalName.UseWeapon, inputMap.ToArray());
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (FilterAllInput)
			return;
		
		DetectAttactInput();
	}
}
