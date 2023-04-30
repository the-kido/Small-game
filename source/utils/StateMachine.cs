using Godot;
using System;
using System.Collections.Generic;



//Make a class which can be instanced once.
//This class can have states ONLY added. These states are just a method. A void method.
//The state machine will handle the switch between states and will call the method synonymous with that state?

public class StateMachine {
    //A list of tuples, where 1 is always the "ready" method and 2 is the "update" method.

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