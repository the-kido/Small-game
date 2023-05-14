using Godot;
using System;
using System.Threading.Tasks;


public sealed class PatrolState : AIState {

    Pathfinder pathfinderComponent;
    int HoverAtSpawnPointDistance;
    public PatrolState(Pathfinder pathfinderComponent, int HoverAtSpawnPointDistance) {
        this.HoverAtSpawnPointDistance = HoverAtSpawnPointDistance;
        this.pathfinderComponent = pathfinderComponent;
        //help this i sbad
    }


    public Action IsIdle;
    public Action IsMoving;

    private Vector2[] goBetween = new Vector2[3];
    int pathOn = 0;
    private State state = State.Walking;

    public override void Init() {
        for (int i = 0; i < 3; i++)
            goBetween[i] = FindValidPatrolPoint();
        
        //SwitchPatrolPoint();
        //Debug
        pathfinderComponent.SetTargetPosition(new(500,5000));
        IsMoving?.Invoke();

    }

    public Vector2 FindValidPatrolPoint() {

        Random rand = new((int)Time.GetTicksUsec());
        
        Vector2 randomDirection = Vector2.Zero;
        //Create a random direction to go (from -1 to 1)
        randomDirection.Y += (float) (rand.NextSingle() - 0.5f) * 2;
        randomDirection.X += (float) (rand.NextSingle() - 0.5f) * 2;

        //Get number between HoverAtSpawnPointDistance/2 and HoverAtSpawnPointDistance
        float range = (rand.NextSingle() / 2 + 0.5f) * HoverAtSpawnPointDistance;

        Vector2 patrolPoint = randomDirection * range;
        
        patrolPoint += actor.GlobalPosition;
        
        return patrolPoint; 
    }
    private enum State {
        Idle,
        Walking
    }


    public async void SwitchPatrolPoint() {
        if (state == State.Idle) return;

        IsIdle?.Invoke();
        //stop its movements.
        // pathfinderComponent.SetTargetPosition(actor.GlobalPosition);

        pathOn = (pathOn + 1) % 3;
        state = State.Idle;

        await Task.Delay(5000);

        stateSwitchCooldown = 0;
        state = State.Walking;
        IsMoving?.Invoke();
        //pathfinderComponent.SetTargetPosition(goBetween[pathOn]);
        pathfinderComponent.SetTargetPosition(FindValidPatrolPoint());
        
    }

    double stateSwitchCooldown = 0;
    public override void Update(double delta) {
        FlipActor();

        stateSwitchCooldown += delta;
        pathfinderComponent.UpdatePathfind(actor);

        if (state == State.Walking) {
            if (stateSwitchCooldown < 1) return;

            if (actor.IsStalling()) {
                actor.Velocity = Vector2.Zero;
                SwitchPatrolPoint(); 
                
                return;
            }
        }

        if (actor.VisiblePlayer() is not null)
            stateMachine.ChangeState(stateToGoTo);
    } 

    private void FlipActor() {
        bool flip = MathF.Sign(actor.Velocity.X) == 1 ? false : true;
        actor.Flip(flip);
    }
}
