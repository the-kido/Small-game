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

    double bloop = 0;
    Player visiblePlayer;

    public override void _Ready() {
        base._Ready();

        stateMachine.AddState(AttackingInit ,AttackingUpdate);
        stateMachine.AddState(PatrollingInit, PatrollingUpdate);

        stateMachine.ChangeState(PatrollingInit);
    }

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
    public override void _Process(double delta) {
        base._Process(delta);
        stateMachine.UpdateState(delta);
    }

    private void PatrollingInit() {
    }

    private void PatrollingUpdate(double delta) {
        pathfinderComponent.PatrolUpdate(delta);

        Player player = VisiblePlayer();
        
        if (player is not null) {
            visiblePlayer = player;
            stateMachine.ChangeState(AttackingInit);
        }
    }

    private void AttackingInit() {
        Velocity = Vector2.Zero;
    }
    private void AttackingUpdate(double delta) {
        bloop += delta;
        if (bloop >= 1) {
            ShootConstantly();
            bloop = 0;
        }

        Player player = VisiblePlayer();

        if (EnemyForgetPlayer(player, delta, ref time)) {
            stateMachine.ChangeState(PatrollingInit);
        }
    }

    private void ShootConstantly() {
        if (visiblePlayer is null) return;

        float angle = (visiblePlayer.GlobalPosition - GlobalPosition).Angle();
        GetNode<BulletFactory>("/root/BulletFactory").SpawnBullet(spamedBullet).init(Position, angle, BulletFrom.Enemy);
    }
}
