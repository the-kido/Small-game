using System;
using Godot;

// [GlobalClass]
public abstract partial class LevelCriteria : Node {    
    public abstract void Start();

    public abstract event Action Finished;
}