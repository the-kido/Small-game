using Godot;

namespace Game.Characters;

public partial class Character : CharacterBody2D {
    [Export]
    public AnimationPlayer AnimationPlayer {get; private set;}

    float speed = 400;
    public bool MoveTo(double delta, Vector2 finalPosition) {
        Vector2 direction = GlobalPosition.DirectionTo(finalPosition);
        Velocity = direction * speed;
        
        MoveAndSlide();

        float distanceToEndPoint = GlobalPosition.DistanceTo(finalPosition);

        return distanceToEndPoint < 5 ? true : false;
    }
}