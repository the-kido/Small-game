using Godot;

public interface IInteractable {

    public bool IsInteractable();
    public Vector2 GetPosition();
    public CollisionShape2D GetCollisionShape();

}