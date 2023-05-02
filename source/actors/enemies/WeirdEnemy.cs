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

    
    //Somehow allow the choice of which state to change to when a state finishes. For instance, will an attacking state
    //always switch to patrolling if it no longer sees a player?
    //I think so, so maybe instancing the AIstate should pass another state which it goes to?
    
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
