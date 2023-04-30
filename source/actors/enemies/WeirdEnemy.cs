using System;
using Godot;
using KidoUtils;
using System.Threading.Tasks;


public enum EnemyStates {
    Patrolling,
    Attacking
}

public partial class WeirdEnemy : Enemy {
    [Export]
    private PackedScene spamedBullet;
    [Export] 
    private Pathfinder pathfinderComponent;

    public EnemyStates state = EnemyStates.Patrolling;
    
    double bloop = 0;
    Player visiblePlayer;


    //Move this into the pathfinder as well.
    
    private bool EnemyForgetPlayer(Player player, double delta, ref float time) {
        if (player is null) {
            
            time += (float) delta;

            if (time > 5) {
                return true;
            }
        }
        else{
            time = 0;
        }
        return false;
    }

    float time = 0;
    public override void _Process(double delta)
    {
        base._Process(delta);
        Player player = VisiblePlayer();

        if (EnemyForgetPlayer(player, delta, ref time)) {
            state = EnemyStates.Patrolling;
        }

        if (player is not null) {
            visiblePlayer = player;
            state = EnemyStates.Attacking;

            //Move this into the pathfinding thing.
            Velocity = Vector2.Zero;
        }

        //TODO: Make the enemy stop following the player after some time.


        if (state == EnemyStates.Attacking) {
            bloop += delta;
            what(visiblePlayer);
        }
        if (state == EnemyStates.Patrolling) {
            pathfinderComponent.PatrolUpdate(delta);
        }


    }
    private void what(Player player) {
        if (bloop >= 1) {
            ShootConstantly(player);
            bloop = 0;
        }
    }
    private void ShootConstantly(Player player) {
        GD.Print(player);

        float angle = (player.GlobalPosition - GlobalPosition).Angle();

        GetNode<BulletFactory>("/root/BulletFactory").SpawnBullet(spamedBullet).init(Position, angle, BulletFrom.Enemy);
    }
}
