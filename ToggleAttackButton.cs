using Godot;
using System;

public partial class ToggleAttackButton : Button {


	public event Action OnAttackButtonPressed;
    public event Action OnMouseEntered;
    public event Action OnMouseExited;
	public override void _Ready() {
		Pressed += OnAttackButtonPressed;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;

	}
 

}
