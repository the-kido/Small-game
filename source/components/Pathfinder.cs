using Godot;

namespace Game.Actors.AI;

public partial class Pathfinder : Node2D {
    [Export]
    private NavigationAgent2D agent;

    public void UpdatePathfind(Actor actor) {
        if (IsNavigationFinished()) {
            actor.Velocity = Vector2.Zero;
            return;
        }
        
        Vector2 direction = GlobalPosition.DirectionTo(GetNextPathPosition());
        actor.Velocity = direction * actor.EffectiveSpeed;
        
    }

    public void temp() => GD.Print(agent.IsTargetReachable());
    public void SetTargetPosition(Vector2 position) => agent.TargetPosition = position;
    public bool IsNavigationFinished() => agent.IsNavigationFinished();
    public Vector2 GetNextPathPosition() => agent.GetNextPathPosition();

}
