using Godot;

namespace Game.Characters;

public partial class Character : CharacterBody2D {
	[Export]
	public AnimationPlayer AnimationPlayer {get; private set;}

	float speed = 400;

	Vector2 velocity;
	public void InitMove(Vector2 finalPosition, float duration) {
		Vector2 direction = GlobalPosition.DirectionTo(finalPosition);
		velocity = direction * (Position.DistanceTo(finalPosition) / duration);
	}

	public void MoveTo() {
		Velocity = velocity;
		
		MoveAndSlide();

		// float distanceToEndPoint = GlobalPosition.DistanceTo(finalPosition);

		// return distanceToEndPoint < 5;
	}

	public void StopMoving() {
		Velocity = Vector2.Zero;
	}
}
