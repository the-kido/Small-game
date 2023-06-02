using Godot;
using KidoUtils;
using System;

// public sealed class DefaultAttackState : AIState {

//     #region events

//     public Action OnShoot;

//     #endregion

   


//     Pathfinder pathfinderComponent;
//     //Not all actors will have pathfinders, so the parameter is necessary.
//     PackedScene spamedBullet;
//     float attackDelay;
//     public DefaultAttackState(Pathfinder pathfinderComponent, PackedScene bullet, float attackDelay) {
//         this.pathfinderComponent = pathfinderComponent;
//         this.spamedBullet = bullet;
//         this.attackDelay = attackDelay;
//     }

//     private bool EnemyForgetPlayer(Player player, double delta, ref float time) {
//         if (player is null) {
//             time += (float) delta;

//             if (time > 10) {
//                 return true;
//             }
//         }
//         else{
//             time = 0;
//         }
//         return false;
//     }

//     public override void Init() {
//         actor.Velocity = Vector2.Zero;
//     }

//     #region wow this is a big huge mess and this SUCKS 
//     float forgetPlayerTimer = 0;
//     double shootTimer = 0;
//     double updatePathfind = 0;
//     double playerSeenTimer;
//     #endregion

//     float stayAwayFromPlayerDistance = 250;
//     float moveAwayFromPlayerDistance = 100;

//     public override void Update(double delta) {
//         //Update the current visible player.
//         Player player = actor.VisiblePlayer();

//         if (player is not null) {
//             lastRememberedPlayer = player;
//             playerSeenTimer += delta;
//         } else {
//             playerSeenTimer = 0;
//         }
        
//         //Update relavent timers
//         updatePathfind += delta;
//         shootTimer += delta;

//         if (updatePathfind > 0.25) {
//             UpdatePathfind(player);
//             updatePathfind = 0;
//         }

//         FlipActor(lastRememberedPlayer);
//         Move(player);

//         if (shootTimer >= attackDelay) {
//             if (playerSeenTimer < 1) return;

//             playerSeenTimer = 0;
//             shootTimer = 0;
//             Shoot(player);
//         }
 
//         if (EnemyForgetPlayer(player, delta, ref forgetPlayerTimer)) {
//             stateMachine.ChangeState(stateToGoTo);
//         }
//     }

//     Player lastRememberedPlayer = Player.players[0];
//     float distanceToPlayer = 0;

//     private void UpdatePathfind(Player currentlyVisiblePlayer) {
//         if (currentlyVisiblePlayer is not null) 
//             distanceToPlayer = actor.GlobalPosition.DistanceTo(lastRememberedPlayer.GlobalPosition);
//         else 
//             distanceToPlayer = 10000;

//         actor.Velocity = Vector2.Zero;
        
//         if (distanceToPlayer > stayAwayFromPlayerDistance) {
//             pathfinderComponent.SetTargetPosition(lastRememberedPlayer.GlobalPosition);
//         }
//     }
//     private void Move(Player visiblePlayer) {
        
//         if (distanceToPlayer > stayAwayFromPlayerDistance || visiblePlayer is null) {
//             pathfinderComponent.UpdatePathfind(actor);
//         }

//         if (distanceToPlayer < moveAwayFromPlayerDistance) {
//             float randFloat = new Random().NextSingle()- 0.5f * 100;
//             actor.Velocity =  lastRememberedPlayer.GlobalPosition.DirectionTo(actor.GlobalPosition + Vector2.One*randFloat) * actor.MoveSpeed*1.5f; 
//         }
//     }
    
//     private void Shoot(Player player) {
//         OnShoot?.Invoke();
//         float angle = (player.GlobalPosition - actor.GlobalPosition).Angle();
//         actor.Velocity =  (actor.GlobalPosition - player.GlobalPosition).Normalized() * 20;
        
//         KidoUtils.Utils.GetPreloadedScene<BulletFactory>(player, PreloadedScene.BulletFactory) 
//             .SpawnBullet(spamedBullet)
//             .init(actor.Position, angle, BulletFrom.Enemy);
//     }
//     private void FlipActor(Player lastRememberedPlayer) {
//         //1 == right, -1 == left.
//         bool flip = MathF.Sign((lastRememberedPlayer.GlobalPosition - actor.GlobalPosition).X) == 1 ? false : true;
//         actor.Flip(flip);
//     }
// }


public sealed class DefaultAttackState : AIState {

    public Action OnShoot;

    DamageInstance damage = new() {
        damage = 5,
        statusEffect = new FireEffect(),
    };
   
   
    Pathfinder pathfinderComponent;
    PackedScene spamedBullet;
    float attackDelay;
    public DefaultAttackState(Pathfinder pathfinderComponent, PackedScene bullet, float attackDelay) {
        this.pathfinderComponent = pathfinderComponent;
        this.spamedBullet = bullet;
        this.attackDelay = attackDelay;
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
                Shoot(player);
            }
        }
        #endregion

        FlipActor(lastRememberedPlayer);
        Move(player);

        
    }

    Player lastRememberedPlayer = Player.players[0];
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
            actor.Velocity =  lastRememberedPlayer.GlobalPosition.DirectionTo(actor.GlobalPosition + Vector2.One*randFloat) * actor.MoveSpeed*1.5f; 
        }
    }
    
    private void Shoot(Player player) {
        OnShoot?.Invoke();
        float angle = (player.GlobalPosition - actor.GlobalPosition).Angle();
        actor.Velocity =  (actor.GlobalPosition - player.GlobalPosition).Normalized() * 20;
        
        KidoUtils.Utils.GetPreloadedScene<BulletFactory>(player, PreloadedScene.BulletFactory) 
            .SpawnBullet(spamedBullet)
            .Init(actor.Position, angle, BulletFrom.Enemy, damage);
    }
    private void FlipActor(Player lastRememberedPlayer) {
        //1 == right, -1 == left.
        bool flip = MathF.Sign((lastRememberedPlayer.GlobalPosition - actor.GlobalPosition).X) == 1 ? false : true;
        actor.Flip(flip);
    }
}


