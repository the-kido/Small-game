using System;
using Godot;

namespace Game.LevelContent.Criteria;

// yes i spelt this wrong i am sorry it should be LevelCriterion but I am too lazy to fix that. 
public abstract partial class LevelCriteria : Node {

    public abstract void Start();

    public abstract event Action Finished;

}