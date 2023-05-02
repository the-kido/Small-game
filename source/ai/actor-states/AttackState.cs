using Godot;
using KidoUtils;
using System;

public sealed class AttackState : AIState {

    Pathfinder pathfinderComponent;
    //Not all actors will have pathfinders, so the parameter is necessary.
    PackedScene spamedBullet;
    public AttackState(Pathfinder pathfinderComponent, PackedScene bullet) {
        this.pathfinderComponent = pathfinderComponent;
        this.spamedBullet = bullet;
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

    public override void Init() {
        actor.Velocity = Vector2.Zero;
    }

    float forgetPlayerTimer = 0;
    double shootTimer = 0;
    float updateDistanceTimer = 0;

    public override void Update(double delta) {
        //Update relavent timers
        updateDistanceTimer += (float) delta;
        shootTimer += delta;

        Player player = actor.VisiblePlayer();

        FinalAttackingMotion(player);

        if (shootTimer >= 0.5f) {
            shootTimer = 0;
            Shoot(player);
        }
 
        if (EnemyForgetPlayer(player, delta, ref forgetPlayerTimer)) {

            stateMachine.ChangeState(stateToGoTo);
        }
    }

    Player lastRememberedPlayer = Player.players[0];


    //Returns the motion while attacking
    private void FinalAttackingMotion(Player player) {
        if (player is not null) {
            lastRememberedPlayer = player;
        }
        
        float distanceToPlayer = actor.GlobalPosition.DistanceTo(lastRememberedPlayer.GlobalPosition);
        if (distanceToPlayer > 250) {
            pathfinderComponent.UpdatePathfind(actor);
        }

        if (updateDistanceTimer < 1) return;
            updateDistanceTimer = 0;
        
        if (distanceToPlayer > 250) {
            pathfinderComponent.SetTargetPosition(lastRememberedPlayer.GlobalPosition);
        }
        else if (player is not null){
            float randFloat = new Random().NextSingle()- 0.5f * 100;
            actor.Velocity = lastRememberedPlayer.GlobalPosition.DirectionTo(actor.GlobalPosition + Vector2.One*randFloat) * actor.MoveSpeed*1.5f;
        }
    }

    private void Shoot(Player player) {
        if (player is null) return;

        float angle = (player.GlobalPosition - actor.GlobalPosition).Angle();
        actor.GetNode<BulletFactory>("/root/BulletFactory").SpawnBullet(spamedBullet).init(actor.Position, angle, BulletFrom.Enemy);
    }
}

