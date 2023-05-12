using Godot;
using KidoUtils;
using System;

public sealed class AttackState : AIState {


    Pathfinder pathfinderComponent;
    //Not all actors will have pathfinders, so the parameter is necessary.
    PackedScene spamedBullet;
    AnimationPlayer animationPlayer;
    public AttackState(Pathfinder pathfinderComponent, PackedScene bullet) {
        this.pathfinderComponent = pathfinderComponent;
        this.spamedBullet = bullet;
    }

    #region events

    public Action OnShoot;

    #endregion

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
    float updatePathfind = 0;

    public override void Update(double delta) {
        //Update relavent timers
        updatePathfind += (float) delta;
        shootTimer += delta;

        Player player = actor.VisiblePlayer();

        if (player is not null) {
            lastRememberedPlayer = player;
        }

        UpdatePathfind();
        FinalAttackingMotion();

        if (shootTimer >= 0.5f) {
            shootTimer = 0;
            OnShoot?.Invoke();        
            Shoot(player);
        }
 
        if (EnemyForgetPlayer(player, delta, ref forgetPlayerTimer)) {

            stateMachine.ChangeState(stateToGoTo);
        }
    }

    Player lastRememberedPlayer = Player.players[0];

    float distanceToPlayer = 0;

    private void UpdatePathfind() {
        
        if (updatePathfind < 0.25f) return;

        distanceToPlayer = actor.GlobalPosition.DistanceTo(lastRememberedPlayer.GlobalPosition);
        updatePathfind = 0;
        actor.Velocity = Vector2.Zero;
        
        if (distanceToPlayer > 250) {
            pathfinderComponent.SetTargetPosition(lastRememberedPlayer.GlobalPosition);
        }
    }
    private void FinalAttackingMotion() {
        
        if (distanceToPlayer > 250) {
            pathfinderComponent.UpdatePathfind(actor);
        }

        if (distanceToPlayer < 100) {
            float randFloat = new Random().NextSingle()- 0.5f * 100;
            actor.Velocity =  lastRememberedPlayer.GlobalPosition.DirectionTo(actor.GlobalPosition + Vector2.One*randFloat) * actor.MoveSpeed*1.5f; 
        }
    }
    

    private void Shoot(Player player) {
        //Make this cleaner, idk if i wanna call this here.

        if (player is null) return;

        float angle = (player.GlobalPosition - actor.GlobalPosition).Angle();
        actor.Velocity =  (player.GlobalPosition - actor.GlobalPosition).Normalized() * 10;
        
        KidoUtils.Utils.GetPreloadedScene<BulletFactory>(player, PreloadedScene.BulletFactory) 
            .SpawnBullet(spamedBullet)
            .init(actor.Position, angle, BulletFrom.Enemy);
    }
}

