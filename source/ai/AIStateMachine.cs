using System.Collections.Generic;
using System;

namespace Game.Actors.AI;

public class AIStateMachine {
    public AIStateMachine(Actor actor) {
        this.actor = actor;
    }

    private AIState currentState;
    private List<AIState> states = new();
    private Actor actor;

    public void AddState(AIState aiState, AIState stateToGoTo) {
        states.Add(aiState);

        //Initialize some of the variables which that state requires.
        aiState.actor = this.actor;
        aiState.stateMachine = this;
        aiState.stateToGoTo = stateToGoTo;
    }
    public void UpdateState(double delta) => currentState?.Update(delta);

    public void ChangeState(AIState aiState)
    {
        if (!states.Contains(aiState))
            throw new Exception($"The method {aiState.ToString()} has not been added to this state machine!");
        currentState = aiState;
        aiState.Init();
    }
}