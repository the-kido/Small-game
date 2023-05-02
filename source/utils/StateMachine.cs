using Godot;
using System;
using System.Collections.Generic;
using KidoUtils;


//Make a class which can be instanced once.
//This class can have states ONLY added. These states are just a method. A void method.
//The state machine will handle the switch between states and will call the method synonymous with that state?




public sealed class AttackState : AIState {

    #region attack state

    Pathfinder pathfinderComponent;
    //Not all actors will have pathfinders, so the parameter is necessary.

    PackedScene spamedBullet;
    public AttackState(Pathfinder pathfinderComponent, PackedScene bullet) {
        this.pathfinderComponent = pathfinderComponent;
        this.spamedBullet = bullet;
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


    //I only want INIT to be called by the stateMachine. How can I fix this issue?
    //Maybe init SHOULD be a lambda which is set by the state machine?
    public override void Init() {
        actor.Velocity = Vector2.Zero;
    }

    float forgetPlayerTimer = 0;
    double shootTimer = 0;
    float updateDistanceTimer = 0;

    public override void Update(double delta) {
        //Update relavent timers
        updateDistanceTimer += (float) delta;
        shootTimer += delta;

        Player player = actor.VisiblePlayer();

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
        
        float distanceToPlayer = actor.GlobalPosition.DistanceTo(lastRememberedPlayer.GlobalPosition);
        if (distanceToPlayer > 250) {
            pathfinderComponent.UpdatePathfind(actor);
        }

        if (updateDistanceTimer < 1) return;
            updateDistanceTimer = 0;
        
        if (distanceToPlayer > 250) {
            pathfinderComponent.SetTargetPosition(lastRememberedPlayer.GlobalPosition);
        }
        else if (player is not null){
            float randFloat = new Random().NextSingle()- 0.5f * 100;
            actor.Velocity = lastRememberedPlayer.GlobalPosition.DirectionTo(actor.GlobalPosition + Vector2.One*randFloat) * actor.MoveSpeed*1.5f;
        }
    }

    private void Shoot(Player player) {
        if (player is null) return;

        float angle = (player.GlobalPosition - actor.GlobalPosition).Angle();
        actor.GetNode<BulletFactory>("/root/BulletFactory").SpawnBullet(spamedBullet).init(actor.Position, angle, BulletFrom.Enemy);
    }

    #endregion
}


public abstract class AIState {
    

    #region ISSUE
    //How can I signify that this will be a solved variable when it's initialized in the state machine?
    //This is very "honours system" rn. 
    public AIState stateToGoTo;
    public AIStateMachine stateMachine;
    public Actor actor;
    #endregion

    public AIState() {}
   
    public abstract void Init();
    public abstract void Update(double delta);
}

public class AIStateMachine {
    public AIStateMachine(Actor actor) {
        this.actor = actor;
    }

    AIState currentState;
    List<AIState> states = new();
    Actor actor;

    public void AddState(AIState aiState, AIState stateToGoTo) {
        states.Add(aiState);
        aiState.actor = this.actor;
        aiState.stateToGoTo = stateToGoTo;
        aiState.stateMachine = this;
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