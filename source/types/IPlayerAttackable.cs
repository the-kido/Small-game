using Godot;

namespace Game.Players;
public interface IPlayerAttackable {
    public bool IsInteractable();
    public Vector2 GetPosition();
    public Node2D GetNode();
    public Rid GetRid();
}