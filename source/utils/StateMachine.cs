using Godot;
using System;
using System.Collections.Generic;



//Make a class which can be instanced once.
//This class can have states ONLY added. These states are just a method. A void method.
//The state machine will handle the switch between states and will call the method synonymous with that state?

public class StateMachine {
    List<Action> states;

    Action currentState;
    public void AddState(Func<object> stateMethod) {

    }
    public void UpdateState() {
        currentState?.Invoke();
        var a = UpdateState;

    }
    public void ChangeState(Action method) {
        if (!states.Contains(method)) {
            GD.PushError("The method ", method, " has not been added to this state machine!");
            return;
        }
        currentState = method;
    }
}