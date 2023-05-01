using System;
using Godot;
using System.Threading.Tasks;

public enum EnemyStates {
    Patrolling,
    Attacking
}

public sealed partial class WeirdEnemy : PatrolEnemy {
    
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
