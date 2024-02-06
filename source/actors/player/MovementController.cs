using Godot;
using Game.Players.Inputs;
using System;

namespace Game.Players;

public partial class MovementController : Node {
	public const int CORNER_CORRECTION_RANGE = 25;
	public event Action PlayerMoved;

	[Export]
	private AnimationTree playerAnimationTree;

	private Player player;
	private InputController inputController;  
	
	private Vector2 previousFramePosition = new();

    private bool GetPlayerIsMoving() {
        if (player.Position == previousFramePosition)
            return false;

        previousFramePosition = player.Position;
        return true;
    }

    public void Init(Player player, InputController inputController) {
		this.player = player;
		this.inputController = inputController;

		// Used in case the player is holding the move button even when the input is frozen. Resets velocity to nothing.
		inputController.UIInputFilter.OnFilterModeChanged += (changed) => 
			player.Velocity = changed ? Vector2.Zero : player.Velocity; 
	}

    private Vector2 GetMovementInput() => inputController.UIInputFilter.FilterNonUiInput
		? Vector2.Zero
		: new Vector2(
			Input.GetAxis("left", "right"),
			Input.GetAxis("up", "down")
		).Normalized();

    public void UpdateMovement() {
		Vector2 normalizedInput = GetMovementInput();
		player.Velocity = normalizedInput * player.EffectiveSpeed * 100;

		if (normalizedInput != Vector2.Zero)
			player.Translate(GetCornerCorrectionOffset(normalizedInput));
		
		PlayMovementAnimations(GetPlayerIsMoving());
		PlayerMoved?.Invoke();
	}

	// I hope to god I never touch this code ever again
	// Thanks celeste
	private Vector2 GetCornerCorrectionOffset(Vector2 movementDirection) {
		if (player.TestMove(player.GlobalTransform, new Vector2(0, movementDirection.Y))) {
			//Find where exactly this object's corner is offseted. 
			//The range (20 — -20) accounts for both left and right shifting
			for (int xOffset = CORNER_CORRECTION_RANGE; xOffset > -CORNER_CORRECTION_RANGE - 1; xOffset -= 5) {
				if (player.TestMove(player.GlobalTransform.Translated(new Vector2(xOffset, 0)), new Vector2(0, movementDirection.Y)))
					continue;
				return new Vector2(xOffset / 5f, 0);
			}
		}
		// Same thing, but for x
		else if (player.TestMove(player.GlobalTransform, new Vector2(movementDirection.X, 0))) {
			//Find where exactly this object's corner is offseted 
			for (int yOffset = CORNER_CORRECTION_RANGE; yOffset > -CORNER_CORRECTION_RANGE - 1; yOffset -= 5) {
				if (player.TestMove(player.GlobalTransform.Translated(new Vector2(0, yOffset)), new Vector2(movementDirection.X, 0)))
					continue;
				return new Vector2(0, yOffset / 5f);
			}
		}
		return Vector2.Zero;
	}

	private void PlayMovementAnimations(bool isMoving) {
		Vector2 playerDirectionNormal = -(player.GlobalPosition - player.GetGlobalMousePosition()).Normalized();

		playerAnimationTree.Set("parameters/conditions/idle", !isMoving);
		playerAnimationTree.Set("parameters/conditions/walk", isMoving);

		playerAnimationTree.Set("parameters/Walk/blend_position", playerDirectionNormal);
		playerAnimationTree.Set("parameters/Idle/blend_position", playerDirectionNormal);
	}
}

