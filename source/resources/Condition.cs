using Godot;
using System;

public partial class Condition : Resource {
    
    // This just depicts which entry into the save file this condition is
    [Export]
    private string Name;
    public Action Achieved;
    public bool IsAchieved {get; private set;} = false;
    
    
    public static Godot.Collections.Dictionary<string, bool> All {get; private set;} = new();
    static Condition() => Level.LevelStarted += Load;
    private static void Load() => All = (Godot.Collections.Dictionary<string,bool>) GameData.GetData()["Conditions"];


    public Condition() {
        Achieved += () => {
            IsAchieved = true;
            All[Name] = true;
        };
    }

    public void Init() {
        if (string.IsNullOrEmpty(Name)) 
            throw new ArgumentNullException("Name for condition must be set!");
        
        IsAchieved = GetAchieved(); 
    }

    private bool GetAchieved() {
        if (All.ContainsKey(Name))
            return All[Name];
        else
            return false;
    }
}


