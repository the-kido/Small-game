using Godot;
using Game.Players;

namespace Game.UI;

public partial class SelectedTargetIndicator : Sprite2D {
	
	static IPlayerAttackable playerTarget;
	static SelectedTargetIndicator currentIndicator;

	private void CreateDuplicate() {
		currentIndicator = (SelectedTargetIndicator) Duplicate();
		playerTarget.GetNode().AddChild(currentIndicator);
		currentIndicator.TreeExited += RemoveDuplicate;
		currentIndicator.Visible = true;
	}
	
	public void Enable(IPlayerAttackable newTarget) {
		RemoveDuplicate();
		playerTarget = newTarget;
		CreateDuplicate();
	}

	public static void RemoveDuplicate() {
		if (currentIndicator is null || playerTarget.IsInteractable()) 
			return;

		currentIndicator.QueueFree();
		currentIndicator = null;
		playerTarget = null;
	}

	public override void _Process(double delta) {
		if (currentIndicator is not null && !playerTarget.IsInteractable())
            RemoveDuplicate();
	}

    public override void _Ready() => Visible = false;

}
