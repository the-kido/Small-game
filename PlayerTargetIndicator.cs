using Godot;
using System;

public partial class PlayerTargetIndicator : Sprite2D
{
	
	IInteractable target;
	public void Enable(bool enable, IInteractable target) {
		this.target = target;
		Visible = enable;
		
	}

	private void UpdatePosition() {
		if (target is null) return;
		
		if (!target.IsInteractable()) {
			Enable(false, null);
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
