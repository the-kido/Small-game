using Godot;
using System;

public partial class MovementController : Node
{
	public const float MOVE_SPEED = 3.0f;
	public const int CORNER_CORRECTION_RANGE = 25;

	[Export] 
	private CharacterBody2D player; 
	[Export]
	private AnimationTree playerAnimationTree;
	
	private Vector2 previousFramePosition = new Vector2();
	private bool PlayerIsMoving {
		get {
			if (player.Position == previousFramePosition) {
				return false;
			}
			previousFramePosition = player.Position;
			return true;
		}
	}


	//Move into Player
	public override void _PhysicsProcess(double delta)
	{
		ControlPlayerMovement();
		PlayMovementAnimations(PlayerIsMoving);
	}

	private void ControlPlayerMovement() {
		var direction = new Vector2(
			Input.GetAxis("left", "right"),
			Input.GetAxis("up", "down")
		).Normalized();

		player.Velocity = direction * MOVE_SPEED * 100;

		player.MoveAndSlide();

		if (direction != Vector2.Zero) {
			CornerCorrection(direction);
		}
	}
	private void CornerCorrection(Vector2 movementDirection) {
		if (player.TestMove(player.GlobalTransform, new Vector2(0, movementDirection.Y))) {
			//Find where exactly this object's corner is offseted. 
			//The range (20 — -20) accounts for both left and right shifting
			for(int xOffset = CORNER_CORRECTION_RANGE; xOffset > -CORNER_CORRECTION_RANGE -1; xOffset -= 5) {
				if (player.TestMove(player.GlobalTransform.Translated(new Vector2(xOffset, 0)), new Vector2(0, movementDirection.Y)))
					continue;
				player.Translate(new Vector2(xOffset/5f, 0));
				return;
			}
		}
		// Same thing, but for x
		else if(player.TestMove(player.GlobalTransform, new Vector2(movementDirection.X, 0)))
		{
			//Find where exactly this object's corner is offseted 
			for(int yOffset = CORNER_CORRECTION_RANGE; yOffset > -CORNER_CORRECTION_RANGE -1; yOffset -= 5) {
				if (player.TestMove(player.GlobalTransform.Translated(new Vector2(0, yOffset)), new Vector2(movementDirection.X, 0)))
					continue;
				player.Translate(new Vector2(0, yOffset/5f));
				return;
			}
		}
	}

	private void PlayMovementAnimations(bool isMoving) {
		Vector2 playerDirectionNormal = -(player.Position - player.GetGlobalMousePosition()).Normalized();
		playerAnimationTree.Set("parameters/conditions/idle", !isMoving);
		playerAnimationTree.Set("parameters/conditions/walk", isMoving);

		playerAnimationTree.Set("parameters/Walk/blend_position", playerDirectionNormal);
		playerAnimationTree.Set("parameters/Idle/blend_position", playerDirectionNormal);
		
	}
}
