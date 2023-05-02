using Godot;

public partial class Pathfinder : Node2D
{
    [Export]
    private NavigationAgent2D agent;

    public void UpdatePathfind(Actor actor) {
        Vector2 direction = GlobalPosition.DirectionTo(GetNextPathPosition());
        actor.Velocity = direction * actor.MoveSpeed;
    }

    public void SetTargetPosition(Vector2 position) => agent.TargetPosition = position;
    public bool IsNavigationFinished() => agent.IsNavigationFinished();
    public Vector2 GetNextPathPosition() => agent.GetNextPathPosition();
}
