using Godot;
using System;
using System.Threading.Tasks;


public sealed class PatrolState : AIState {

    Pathfinder pathfinderComponent;
    int HoverAtSpawnPointDistance;
    public PatrolState(Pathfinder pathfinderComponent, int HoverAtSpawnPointDistance) {
        this.HoverAtSpawnPointDistance = HoverAtSpawnPointDistance;
        this.pathfinderComponent = pathfinderComponent;
    }

    private Vector2[] goBetween = new Vector2[3];
    int pathOn = 0;
    private State state = State.Walking;

    public override void Init() {
        for (int i = 0; i < 3; i++)
            goBetween[i] = FindValidPatrolPoint();
        
        //Debug
        goBetween[0] = new Vector2(500, 5000);
        //Debug

        pathfinderComponent.SetTargetPosition(goBetween[0]);
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


    volatile bool isSwitching = false;
    public async void SwitchPatrolPoint() {

        if (isSwitching) return;

        pathOn = (pathOn + 1) % 3;
        state = State.Idle;

        isSwitching = true;
        await Task.Delay(5000);
        isSwitching = false;

        state = State.Walking;
        //pathfinderComponent.SetTargetPosition(goBetween[pathOn]);
        pathfinderComponent.SetTargetPosition(FindValidPatrolPoint());
        
    }

    float stallingTimer = 0;
    public override void Update(double delta) {

        if (state == State.Walking) {
            if (pathfinderComponent.IsNavigationFinished() || actor.IsStalling(delta, 1, ref stallingTimer) == true) {
                actor.Velocity = Vector2.Zero;
                SwitchPatrolPoint(); 
                return;
            }
        }
        pathfinderComponent.UpdatePathfind(actor);

        if (actor.VisiblePlayer() is not null)
            stateMachine.ChangeState(stateToGoTo);
    } 
}
