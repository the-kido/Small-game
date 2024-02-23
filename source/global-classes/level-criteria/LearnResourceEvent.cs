using System;
using Game.Data;
using Godot;

namespace Game.LevelContent.Criteria;

[GlobalClass]
public partial class LearnResourceEvent : LevelCriteria {
    public override event Action Finished;
    [Export]
    RunDataEnum runDataEnum;

    public override void Start() {
        ResourcesViewer.DiscoverNewResource(runDataEnum);
        Finished?.Invoke(); 
    }
}
