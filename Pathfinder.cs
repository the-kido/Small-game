using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Pathfinder : Node2D
{
    [Export]
    private NavigationAgent2D agent;

    public void UpdatePathfind(Actor actor) {
        var direction = GlobalPosition.DirectionTo(GetNextPathPosition());
        actor.Velocity = direction * actor.MoveSpeed;
    }

    public void SetTargetPosition(Vector2 position) {
        agent.TargetPosition = position;
        
    }
    public bool IsNavigationFinished() {
        return agent.IsNavigationFinished();
    }
    public Vector2 GetNextPathPosition() {
        return agent.GetNextPathPosition();
    }
}

public class tempAI {

}
