using Godot;

public partial class Character : CharacterBody2D {
    [Export]
    public AnimationPlayer AnimationPlayer {get; private set;}

    public void MoveTo(Vector2 finalPosition) {
        
    }
}