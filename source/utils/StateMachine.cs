using Godot;
using System;
using System.Collections.Generic;



//Make a class which can be instanced once.
//This class can have states ONLY added. These states are just a method. A void method.
//The state machine will handle the switch between states and will call the method synonymous with that state?




public sealed class AttackState : AIState {

    #region attack state

    public AttackState(AIState stateToGoTo, AIStateMachine stateMachine) : base(stateToGoTo, stateMachine) {
        this.stateToGoTo = stateToGoTo;
        this.stateMachine = stateMachine;
    }

    protected override void Init() {
        Velocity = Vector2.Zero;
    }

    float forgetPlayerTimer = 0;
    double shootTimer = 0;
    float updateDistanceTimer = 0;

    protected override void Update(double delta) {
        //Update relavent timers
        updateDistanceTimer += (float) delta;
        shootTimer += delta;

        Player player = VisiblePlayer();

        FinalAttackingMotion(player);

        if (shootTimer >= 0.5f) {
            shootTimer = 0;
            Shoot(player);
        }
 
        if (EnemyForgetPlayer(player, delta, ref forgetPlayerTimer)) {

            stateMachine.ChangeState(stateToGoTo);
        }
    }

    Player lastRememberedPlayer = Player.players[0];


    //Returns the motion while attacking
    private void FinalAttackingMotion(Player player) {
        if (player is not null) {
            lastRememberedPlayer = player;
        }
        
        float distanceToPlayer = GlobalPosition.DistanceTo(lastRememberedPlayer.GlobalPosition);
        if (distanceToPlayer > 250) {
            pathfinderComponent.UpdatePathfind(this);
        }

        if (updateDistanceTimer < 1) return;
            updateDistanceTimer = 0;
        
        if (distanceToPlayer > 250) {
            pathfinderComponent.SetTargetPosition(lastRememberedPlayer.GlobalPosition);
        }
        else if (player is not null){
            float randFloat = new Random().NextSingle()- 0.5f * 100;
            Velocity = lastRememberedPlayer.GlobalPosition.DirectionTo(GlobalPosition + Vector2.One*randFloat) * MoveSpeed*1.5f;
        }
    }

    private void Shoot(Player player) {
        if (player is null) return;

        float angle = (player.GlobalPosition - GlobalPosition).Angle();
        GetNode<BulletFactory>("/root/BulletFactory").SpawnBullet(spamedBullet).init(Position, angle, BulletFrom.Enemy);
    }

    #endregion
}


public abstract class AIState {
    protected AIState stateToGoTo;
    protected AIStateMachine stateMachine;
    public AIState(AIState stateToGoTo, AIStateMachine stateMachine) {
        this.stateToGoTo = stateToGoTo;
        this.stateMachine = stateMachine;
    }
   
    protected abstract void Init();
    protected abstract void Update(double delta);
}

public class AIStateMachine {
    AIState currentState;
    List<AIState> states;

    public void AddState(AIState aiState) {
        states.Add(aiState);
    }
    public void UpdateState(double delta) {
        currentState.Update(delta);
    }
    //How do I avoid this? Is this approach bad?!
    public void ChangeState(AIState aiState)
    {
        if (!states.Contains(aiState))
            throw new Exception($"The method {aiState.ToString()} has not been added to this state machine!");
        currentState = aiState;
        aiState.Init();
    }

}

public class StateMachine {
    Dictionary<Action, Action<double>> states = new();

    Action currentInitialMethod;
    Action<double> currentUpdateMethod;

    public void AddState(Action initial, Action<double> update) {
        states.Add(initial, update);
        
    }
    public void UpdateState(double delta) {
        currentUpdateMethod?.Invoke(delta);
    }

    public void ChangeState(Action initial) {

        if (!states.ContainsKey(initial))
            throw new Exception($"The method {initial.ToString()} has not been added to this state machine!");

        currentUpdateMethod = states[initial];
        currentInitialMethod = initial;
        currentInitialMethod?.Invoke();

    }
}