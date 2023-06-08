using Godot;
using System;

public partial class SelectedTargetIndicator : Sprite2D {
	
	IInteractable target;
	public void Enable(IInteractable target) {
		this.target = target;
		Visible = true;
	}

	public void Disable() {
		this.target = null;
		Visible = false;
	}

	private void UpdatePosition() {
		if (target is null) return;
		
		if (!target.IsInteractable()) {
			Disable();
			return;
		}

		Vector2 offset = Vector2.One;
		offset.Y *= -50;
		Position = target.GetPosition() + offset;
	}
	public override void _Process(double delta) {
		UpdatePosition();
	}

    public override void _Ready() {
		Visible = false;
    }
}
