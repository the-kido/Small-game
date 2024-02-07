using Godot;
using KidoUtils;
using System;
using Game.ActorStatuses;
using Game.Players;
using Game.Damage;
using Game.Bullets;
using Game.Autoload;

namespace Game.Actors.AI;

public sealed class DefaultAttackState : AIState {

    public Action OnShoot;

    DamageInstance Damage => new(actor) {
        damage = 5,
        statusEffect = new FireEffect(),
    };
    
    Pathfinder pathfinderComponent;
    PackedScene spamedBullet;
    float attackDelay;
    public DefaultAttackState(Pathfinder pathfinderComponent, PackedScene bullet, float attackDelay) {
        this.pathfinderComponent = pathfinderComponent;
        spamedBullet = bullet;
        this.attackDelay = attackDelay;
    }

    private bool EnemyForgetPlayer(Player player, double delta, ref float time) {
        if (player is null) {
            time += (float) delta;

            if (time > 5) {
                GD.Print("i forgor");
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


    #region wow this is a big huge mess and this SUCKS 
    
    #endregion


    //this is the closest the enemy will get to the player before stopping.
    float stayAwayFromPlayerDistance = 250;
    //if the player gets too close, it backs off. 
    float moveAwayFromPlayerDistance = 100;


    //Timers
    double shootTimer;
    double updatePathfindTimer;
    float forgetPlayerTimer;
    double playerSeenDuration;

    public override void Update(double delta) {
        //Update the current visible player.
        Player player = actor.VisiblePlayer();

        if (player is not null) {
            lastRememberedPlayer = player;
            playerSeenDuration += delta;
        }

        if (EnemyForgetPlayer(player, delta, ref forgetPlayerTimer)) {
            stateMachine.ChangeState(stateToGoTo);
        }

        #region timers
        updatePathfindTimer += delta;
        if (updatePathfindTimer > 0.25) {
            UpdatePathfind(player);
        }

        shootTimer += delta;
        if (shootTimer >= attackDelay) {
            if (playerSeenDuration > 1) {
                playerSeenDuration = 0;

                shootTimer = 0;
                actor.Velocity = Vector2.Zero;
                
                if (player is not null) Shoot(player);
            }
        }
        #endregion

        FlipActor(lastRememberedPlayer);
        Move(player);
    }

    Player lastRememberedPlayer = null;
    float distanceToPlayer = 0;

    private void UpdatePathfind(Player currentlyVisiblePlayer) {
        actor.Velocity = Vector2.Zero;
        if (currentlyVisiblePlayer is not null) 
            distanceToPlayer = actor.GlobalPosition.DistanceTo(lastRememberedPlayer.GlobalPosition);
        else 
            distanceToPlayer = 10000;
        
        if (distanceToPlayer > stayAwayFromPlayerDistance) {
            pathfinderComponent.SetTargetPosition(lastRememberedPlayer.GlobalPosition);
        }
    }
    private void Move(Player visiblePlayer) {
        
        if (distanceToPlayer > stayAwayFromPlayerDistance || playerSeenDuration < 1) {
            pathfinderComponent.UpdatePathfind(actor);
        }

        if (distanceToPlayer < moveAwayFromPlayerDistance) {
            float randFloat = new Random().NextSingle() - 0.5f * 100;
            actor.Velocity =  lastRememberedPlayer.GlobalPosition.DirectionTo(actor.GlobalPosition + Vector2.One*randFloat) * actor.EffectiveSpeed*1.5f; 
        }
    }
    
    BulletInstance bulletInstance => new(BulletFrom.Enemy, Damage, BulletSpeed.KindaSlow);

    private void Shoot(Player player) {
        OnShoot?.Invoke();
        float angle = (player.GlobalPosition - actor.GlobalPosition).Angle();
        actor.Velocity =  (actor.GlobalPosition - player.GlobalPosition).Normalized() * 20;
        
        Utils.GetPreloadedScene<BulletFactory>(player, PreloadedScene.BulletFactory) 
            .SpawnBullet(spamedBullet)
            .Init(actor.Position, angle, bulletInstance);
    }
    private void FlipActor(Player lastRememberedPlayer) {
        //1 == right, -1 == left.
        bool flip = MathF.Sign((lastRememberedPlayer.GlobalPosition - actor.GlobalPosition).X) != 1;
        actor.Flip(flip);
    }
}


