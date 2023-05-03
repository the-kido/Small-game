using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class InputController : Node
{
	public event Action<List<InputType>> UseWeapon;

	// [Signal]
	// public delegate void UseWeaponEventHandler(string[] inputMap);
	
	public bool FilterAllInput {set; get;} = false;


	#region All attack related input methods 
	private void DetectAttackInput() {

		List<InputType> inputMap = new();

		if (Input.IsActionPressed("default_attack")) {
			inputMap.Add(InputType.LeftClick);
		}
		if (inputMap.Count > 0) {
			OnAttackKeyHeld(inputMap);
		}
	}
	private bool reloaded = true;
	private async void OnAttackKeyHeld(List<InputType> inputMap) {
		if (!reloaded)
			return;
		
		reloaded = false;
		UseWeapon?.Invoke(inputMap);

		// EmitSignal(SignalName.UseWeapon, inputMap);

		await Task.Delay((int)(GetNode("../Hand").GetNode<Weapon>("Weapon").ReloadSpeed * 1000));

		reloaded = true;
	}

	#endregion 

	public override void _Process(double delta)
	{
		if (FilterAllInput)
			return;
		
		DetectAttackInput();
	}
}

public enum InputType {
	LeftClick,
	RightClick,
}