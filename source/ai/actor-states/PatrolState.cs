using Godot;
using System;

namespace Game.Actors.AI;

public sealed class PatrolState : AIState {

    readonly Pathfinder pathfinderComponent;
    readonly int hoverAtSpawnPointDistance;
    public PatrolState(Pathfinder pathfinderComponent, int hoverAtSpawnPointDistance) {
        this.hoverAtSpawnPointDistance = hoverAtSpawnPointDistance;
        this.pathfinderComponent = pathfinderComponent;

        idleTimer.TimeOver += StopIdling;
    }

    public Action IsIdle;
    public Action IsMoving;

    private State state = State.Walking;

    public override void Init() {
        pathfinderComponent.SetTargetPosition(FindValidPatrolPoint());
        IsMoving?.Invoke();
    }

    public Vector2 FindValidPatrolPoint() {

        Random rand = new((int)Time.GetTicksUsec());
        
        //Get number between HoverAtSpawnPointDistance/2 and HoverAtSpawnPointDistance
        float range = (rand.NextSingle() / 2 + 0.5f) * hoverAtSpawnPointDistance;

        
        Vector2 patrolPoint = Vector2.One.Random() * range;
        
        patrolPoint += actor.GlobalPosition;
        
        return patrolPoint; 
    }

    private enum State {
        Idle,
        Walking
    }


    KidoUtils.Timer idleTimer = new(5);// KidoUtils.Timer.NONE;
    public void Idle() {
        if (state == State.Idle) return;

        pathfinderComponent.SetTargetPosition(actor.GlobalPosition); // Stop them from conitnuing their navigation
        IsIdle?.Invoke();
        state = State.Idle;
        idleTimer.Reset();
    }

    private void StopIdling() {
        pathfinderComponent.SetTargetPosition(FindValidPatrolPoint());
        
        pathfinderComponent.temp();
        
        state = State.Walking;
        IsMoving?.Invoke();
    
        buffer = 0;

    }


    double buffer = 0;
    public override void Update(double delta) {
        FlipActor();
        buffer += delta;

        if (state is State.Idle) idleTimer.Update(delta);

        pathfinderComponent.UpdatePathfind(actor);

        if (state is State.Walking) {
            // lets character be like "oh shoot i gotta move" instead of giving up on the spot.
            if (buffer < 1) return;
            
            if (actor.IsStalling()) {
                actor.Velocity = Vector2.Zero;
                Idle(); 
            }
        }

        // Stop patrolling after finding a player.
        if (actor.VisiblePlayer() is not null) stateMachine.ChangeState(stateToGoTo);
    } 

    private void FlipActor() {
        int val = MathF.Sign(actor.Velocity.X);
        
        if (val is not 0) actor.Flip(val is -1);
    }
}