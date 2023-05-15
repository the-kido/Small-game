using Godot;
using KidoUtils;
using System;

public sealed class DefaultAttackState : AIState {


    Pathfinder pathfinderComponent;
    //Not all actors will have pathfinders, so the parameter is necessary.
    PackedScene spamedBullet;
    float attackDelay;
    public DefaultAttackState(Pathfinder pathfinderComponent, PackedScene bullet, float attackDelay) {
        this.pathfinderComponent = pathfinderComponent;
        this.spamedBullet = bullet;
        this.attackDelay = attackDelay;
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
        FinalAttackingMotion(player);
        FlipActor(lastRememberedPlayer);

        if (shootTimer >= attackDelay) {
            
            if (player is not null) {
                shootTimer = 0;
                Shoot(player);
            }
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
    private void FinalAttackingMotion(Player visiblePlayer) {
        
        if (distanceToPlayer > 250 || visiblePlayer is null) {
            pathfinderComponent.UpdatePathfind(actor);
        }

        if (distanceToPlayer < 100) {
            float randFloat = new Random().NextSingle()- 0.5f * 100;
            actor.Velocity =  lastRememberedPlayer.GlobalPosition.DirectionTo(actor.GlobalPosition + Vector2.One*randFloat) * actor.MoveSpeed*1.5f; 
        }
    }
    
    private void Shoot(Player player) {
        OnShoot?.Invoke();

        float angle = (player.GlobalPosition - actor.GlobalPosition).Angle();
        actor.Velocity =  (actor.GlobalPosition - player.GlobalPosition).Normalized() * 20;
        
        KidoUtils.Utils.GetPreloadedScene<BulletFactory>(player, PreloadedScene.BulletFactory) 
            .SpawnBullet(spamedBullet)
            .init(actor.Position, angle, BulletFrom.Enemy);
    }
    private void FlipActor(Player lastRememberedPlayer) {
        //1 == right, -1 == left.
        bool flip = MathF.Sign((lastRememberedPlayer.GlobalPosition - actor.GlobalPosition).X) == 1 ? false : true;
        actor.Flip(flip);
    }
}


