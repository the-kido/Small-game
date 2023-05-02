using System;
using Godot;
using System.Threading.Tasks;

public enum EnemyStates {
    Patrolling,
    Attacking
}

public sealed partial class WeirdEnemy : Enemy {
    [Export] 
    public Pathfinder pathfinderComponent;
    [Export]
    private int HoverAtSpawnPointDistance = 0;
    [Export]
    private PackedScene spamedBullet;

    public override void _Ready() {
        base._Ready();
        
        AttackState attackState = new(pathfinderComponent, spamedBullet);
        PatrolState patrolState = new(pathfinderComponent, HoverAtSpawnPointDistance);

        stateMachine.AddState(attackState, patrolState);
        stateMachine.AddState(patrolState, attackState);

        stateMachine.ChangeState(patrolState);
    }

    public override void _Process(double delta) {
        base._Process(delta);
        stateMachine.UpdateState(delta);
    }
}
