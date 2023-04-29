using Godot;
using System;
using System.Collections.Generic;

public partial class Pathfinder : Node2D
{
    [Export]
    private int HoverAtSpawnPointDistance = 0;
    [Export]
    private Actor actor;
  
    [Export]
    private NavigationAgent2D agent;

    private Vector2 CONST = new(500, 300);

    public override void _Ready() {
        agent.TargetPosition = new Vector2(500,300);
    }

    int pathOn;
    public override void _Process(double delta) {

        agent.GetCurrentNavigationPath();

        if (agent.IsNavigationFinished()) {
            actor.Velocity = Vector2.Zero;
            return;
        }
        var direction = GlobalPosition.DirectionTo(agent.GetNextPathPosition());
        actor.Velocity = direction * 200;

    }

    // public override void _Ready() {
    //     if (actor is Player player) {
    //         throw new ArrayTypeMismatchException("Players cannot have the Pathfinder component!");
    //     }

    //     spawnPoint = GlobalPosition;
    // }

    // double stopTime = 0;
    // private void Patrol(double delta) {
    //     Vector2 goTo = Vector2.Zero;
    //     Vector2 movementToPoint = actor.Velocity;

    //     timeToMove += delta;
    //     stopTime -= delta;
    //     if (timeToMove >= 1.5) {
    //         goTo = FindNewPatrolPoint(true, out movementToPoint, out stopTime);
    //         timeToMove = 0;
    //         actor.Velocity = movementToPoint;
    //     }

    //     if (stopTime <= 0) {
    //         actor.Velocity = Vector2.Zero;
    //     }
        
    //     if (actor.GlobalPosition.DistanceTo(goTo) < 10) {
    //         actor.Velocity = Vector2.Zero;
    //         goTo = Vector2.Zero;
    //     }
    //     if (actor.Velocity != movementToPoint) {
    //         actor.Velocity = Vector2.Zero;
    //         FindNewPatrolPoint(true, out movementToPoint, out stopTime);
    //         actor.Velocity = movementToPoint;
    //     }
    // }

    // public override void _Process(double delta) {
    //    Patrol(delta);
    // }       
    
    // int recurrsionCount = 0;
    // private Vector2 FindNewPatrolPoint(bool isPatrolling, out Vector2 directionToPoint, out double stopTime) {
        
    //     Random rand = new((int)Time.GetTicksUsec());
        
    //     Vector2 randomDirection = Vector2.One;
    //     //Create a random direction to go.
    //     randomDirection.Y *= (float) (rand.NextDouble() - 0.5f) * HoverAtSpawnPointDistance * 2;
    //     randomDirection.X += (float) (rand.NextDouble() - 0.5f) * HoverAtSpawnPointDistance * 2;
        
    //     //Add that offset to the character's patrol point.

    //     Vector2 shiftTo = randomDirection;
    //     shiftTo += isPatrolling ? spawnPoint : GlobalPosition;

    //     stopTime = 1.5;
    //     directionToPoint = GlobalPosition.DirectionTo(shiftTo).Normalized() * actor.MoveSpeed;

    //     if (actor.TestMove(actor.GlobalTransform, directionToPoint * actor.MoveSpeed) && recurrsionCount <= 10) {
    //         recurrsionCount++;
    //         FindNewPatrolPoint(true, out var _, out var _);
    //     }

    //     recurrsionCount = 0;

    //     // if (actor.GlobalPosition.DistanceTo(goTo) < 50) {
    //     //     FindNewPatrolPoint(true);
    //     // }

        
    //     return shiftTo;
        
    // }
    
    
}
