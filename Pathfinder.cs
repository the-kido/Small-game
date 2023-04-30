using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Pathfinder : Node2D
{
    [Export]
    private int HoverAtSpawnPointDistance = 0;
    [Export]
    private Actor actor;
    [Export]
    private NavigationAgent2D agent;

    int pathOn = 0;
    private Vector2[] goBetween = new Vector2[3];
    private State state = State.Walking;

    public override void _Ready() {
        for (int i = 0; i < 3; i++)
            goBetween[i] = FindValidPatrolPoint();
        
        agent.TargetPosition = goBetween[0];
    }

    public Vector2 FindValidPatrolPoint() {
        Vector2 patrolPoint;

        Random rand = new((int)Time.GetTicksUsec());
        
        Vector2 randomDirection = Vector2.Zero;
        //Create a random direction to go (from -1 to 1)
        randomDirection.Y += (float) (rand.NextSingle() - 0.5f) * 2;
        randomDirection.X += (float) (rand.NextSingle() - 0.5f) * 2;

        //Get number between HoverAtSpawnPointDistance/2 and HoverAtSpawnPointDistance
        float range = (rand.NextSingle() / 2 + 0.5f) * HoverAtSpawnPointDistance;

        patrolPoint = randomDirection * range;
        patrolPoint += GlobalPosition;
        
        return patrolPoint; 
    }

    float stalledFor = 0;
    public bool IsStalling(double delta) {
        if (previousVelocity == actor.Velocity) {
            
            stalledFor += (float) delta;

            if (stalledFor > 1) {
                return true;
            }
        }
        else{
            stalledFor = 0;
        }

        previousVelocity = actor.Velocity;
        return false;
    }
    Vector2 previousVelocity;

    public async void SwitchPatrolPoint() {
        pathOn = (pathOn + 1) % 3;
        state = State.Idle;

        await Task.Delay(5000);

        state = State.Walking;
        agent.TargetPosition = goBetween[pathOn];
        
    }

    public void MoveActorToNextPosition() {
        var direction = GlobalPosition.DirectionTo(agent.GetNextPathPosition());
        actor.Velocity = direction * 200;
    }

    public void PatrolUpdate(double delta) {
        if (state == State.Walking) {
            if (agent.IsNavigationFinished() || IsStalling(delta) == true) {
                actor.Velocity = Vector2.Zero;
                SwitchPatrolPoint(); 
                return;
            }
            MoveActorToNextPosition();
        }
    }
    private enum State {
        Idle,
        Walking
    }
}

