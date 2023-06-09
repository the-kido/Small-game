using Godot;
using System;

public partial class Condition : Resource {

    public bool achieved = false;
    public Action OnConditionAchieved;
    Condition() =>
        OnConditionAchieved += () => achieved = true;

}
