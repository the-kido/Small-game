using System;
using Godot;
using System.Threading.Tasks;

public enum EnemyStates {
    Patrolling,
    Attacking
}

public sealed partial class WeirdEnemy : PatrolEnemy {
    
    //Somehow allow the choice of which state to change to when a state finishes. For instance, will an attacking state
    //always switch to patrolling if it no longer sees a player?
    //I think so, so maybe instancing the AIstate should pass another state which it goes to?
    
    public override void _Ready() {
        base._Ready();

        stateMachine.AddState(AttackingInit, AttackingUpdate);
        stateMachine.AddState(PatrollingInit, PatrollingUpdate);

        stateMachine.ChangeState(PatrollingInit);
    }

    public override void _Process(double delta) {
        base._Process(delta);
        stateMachine.UpdateState(delta);
    }
}
