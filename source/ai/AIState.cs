using Godot;
using System;
using System.Collections.Generic;
using KidoUtils;

public abstract class AIState {
    public AIState stateToGoTo;
    public AIStateMachine stateMachine;
    public Actor actor;

    public abstract void Init();
    public abstract void Update(double delta);
}
