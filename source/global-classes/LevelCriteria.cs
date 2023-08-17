using System;
using Godot;

// [GlobalClass]
namespace Game.LevelContent.Criteria;

public abstract partial class LevelCriteria : Node {    
    public abstract void Start();

    public abstract event Action Finished;
}