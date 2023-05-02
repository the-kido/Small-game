using Godot;
using System;
using System.Threading.Tasks;
using KidoUtils;


public abstract partial class PatrolEnemy : Enemy {

    [Export] 
    public Pathfinder pathfinderComponent;
    [Export]
    private int HoverAtSpawnPointDistance = 0;
    [Export]
    private PackedScene spamedBullet;

    private Vector2[] goBetween = new Vector2[3];
    int pathOn = 0;
    private State state = State.Walking;

    public void PatrollingInit() {
        for (int i = 0; i < 3; i++)
            goBetween[i] = FindValidPatrolPoint();
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
        patrolPoint += GlobalPosition;
        
        return patrolPoint; 
    }
    private enum State {
        Idle,
        Walking
    }

    public async void SwitchPatrolPoint() {
        pathOn = (pathOn + 1) % 3;
        state = State.Idle;

        await Task.Delay(5000);

        state = State.Walking;
        pathfinderComponent.SetTargetPosition(goBetween[pathOn]);
    }

    float stallingTimer = 0;
    protected virtual void PatrollingUpdate(double delta) {

        if (state == State.Walking) {
            if (pathfinderComponent.IsNavigationFinished() || IsStalling(delta, 1, ref stallingTimer) == true) {
                Velocity = Vector2.Zero;
                SwitchPatrolPoint(); 
                return;
            }
            pathfinderComponent.UpdatePathfind(this);
        }

        if (VisiblePlayer() is not null)
            stateMachine.ChangeState(AttackingInit);
    } 

    
    



    private bool EnemyForgetPlayer(Player player, double delta, ref float time) {
        if (player is null) {
            time += (float) delta;

            if (time > 10) {
                return true;
            }
        }
        else{
            time = 0;
        }

        return false;
    }
  
}
