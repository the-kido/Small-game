using Godot;
using System;
using System.Collections.Generic;


public partial class Condition : Resource, ISaveable {
    
    // This just depicts which entry into the save file this condition is
    [Export]
    private string Name;
    public Action Achieved;
    public bool IsAchieved {get; private set;} = false;
    
    
    public static Godot.Collections.Dictionary<string, bool> All {get; private set;} = new();

    public string SaveKey => "Conditions";
    public Variant SaveValue => All;

    static Condition() => Level.LevelStarted += Load;
    private static void Load() => All = (Godot.Collections.Dictionary<string,bool>) GameDataService.GetData()["Conditions"];

    public Condition() {
        Level.LevelStarted += (this as ISaveable).InitSaveable;
        All = (Godot.Collections.Dictionary<string, bool>) (this as ISaveable).LoadData();
        
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


