using System.Collections.Generic;
using System;
using Godot;

namespace Game.Actors.AI;

public class AIStateMachine {
    public AIStateMachine(Actor actor) {
        this.actor = actor;
    }

    private AIState currentState;
    private readonly List<AIState> states = new();
    private readonly Actor actor;

    public void AddState(AIState aiState, AIState stateToGoTo) {
        states.Add(aiState);

        //Initialize some of the variables which that state requires.
        aiState.actor = actor;
        aiState.stateMachine = this;
        aiState.stateToGoTo = stateToGoTo;
    }

    public void UpdateState(double delta) => currentState?.Update(delta);

    public void ChangeState(AIState aiState) {
        GD.Print("CHanging states to" + aiState.GetType().ToString());
        if (!states.Contains(aiState))
            throw new Exception($"The method {aiState} has not been added to this state machine!");
        currentState = aiState;
        aiState.Init();
    }
}