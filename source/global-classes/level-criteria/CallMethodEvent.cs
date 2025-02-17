using System;
using Godot;

namespace Game.LevelContent.Criteria;

[GlobalClass]
public partial class CallMethodEvent : LevelCriteria {
    public override event Action Finished;
    [Export]
    Node invokedNode;
    [Export]
    StringName methodName;

    public override void Start() {
        invokedNode.CallDeferred(methodName);
        Finished?.Invoke();
    }
}
