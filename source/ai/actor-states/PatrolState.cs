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

    private enum State { Idle, Walking }
    private State state = State.Walking;

    private KidoUtils.Timer idleTimer = new(5);

    public override void Init() {
        pathfinderComponent.SetTargetPosition(FindValidPatrolPoint());
        IsMoving?.Invoke();
    }

    public Vector2 FindValidPatrolPoint() {
        Random rand = new((int)Time.GetTicksUsec());
        
        //Get number between HoverAtSpawnPointDistance/2 and HoverAtSpawnPointDistance
        float range = rand.NextSingle() / 2 + 0.5f;

        Vector2 patrolPoint = Vector2.One.Random() * range * hoverAtSpawnPointDistance;
        
        patrolPoint += actor.GlobalPosition;
        
        return patrolPoint; 
    }

    public void Idle() {
        if (state == State.Idle) return;

        pathfinderComponent.SetTargetPosition(actor.GlobalPosition); // Stop them from conitnuing their navigation
        IsIdle?.Invoke();
        state = State.Idle;
        idleTimer.Reset();
    }

    private void StopIdling() {
        pathfinderComponent.SetTargetPosition(FindValidPatrolPoint());
        
        state = State.Walking;
        IsMoving?.Invoke();
    
        moveBuffer = 0;
    }

    /// <summary>
    /// Right after setting the new positon, the actor will assume it hasn't moved and is therefore stalling
    /// So this buffer is a countermeasure to allow the actor to correct itself.
    /// </summary>
    double moveBuffer = 0;
    public override void Update(double delta) {
        FlipActor();
        moveBuffer += delta;
        pathfinderComponent.UpdatePathfind(actor);

        if (state is State.Idle) idleTimer.Update(delta);

        // Buffer allows actor to move before assuming it's already stalling
        if (state is State.Walking && moveBuffer > 1 && actor.IsStalling()) {
            actor.Velocity = Vector2.Zero;
            Idle(); 
        }

        // Stop patrolling after finding a player.
        if (actor.VisiblePlayer() is not null) stateMachine.ChangeState(stateToGoTo);
    } 

    private void FlipActor() {
        int val = MathF.Sign(actor.Velocity.X);
        
        if (val is not 0) actor.Flip(val is -1);
    }
}