using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class InputController : Node
{
	//MAYBE Delegate this to another class for player GUI management? Idk tbh tho
	[Export]
	private PlayerHUD playerHUD;

	public event Action<List<InputType>> UpdateWeapon;

	public bool FilterAllInput {get; private set;} = false;
	private bool isAttackPressed = false;
	public override void _Ready() {
		playerHUD.OnAttackButtonPressed += () => isAttackPressed = !isAttackPressed;
	}

	#region All attack related input methods 
	private void UpdateAttackInput() {

		List<InputType> inputMap = new();

		if (isAttackPressed) {
			inputMap.Add(InputType.AttackButtonPressed);
		}
		if (Input.IsActionPressed("default_attack")) {
			inputMap.Add(InputType.LeftClick);
		}

		UpdateWeapon?.Invoke(inputMap);
		//OnAttackKeyHeld(inputMap);

	}
	

	#endregion 

	public override void _Process(double delta)
	{
		if (FilterAllInput)
			return;
		
		UpdateAttackInput();
	}
}

public enum InputType {
	LeftClick,
	RightClick,
	AttackButtonPressed,
}