using Godot;
using Godot.Collections;
using System;
using System.ComponentModel;
using Game.Data;
using Game.LevelContent;

public partial class Condition : Resource, ISaveable {
    
    [Export, Description("Required to differentiate condition entries in the save file")]
    private string Name;
    public Action Achieved;
    public bool IsAchieved {get; private set;} = false;
    
    // Required for saving the conditions
    public static Dictionary<string, bool> All {get; private set;} = new();
    public SaveData saveData => new("Conditions", All);
    private static void Load() => All = (Dictionary<string,bool>) GameDataService.GetData()["Conditions"];
    private bool GetAchieved() => All.ContainsKey(Name) ? All[Name] : false;
    static Condition() => Level.LevelStarted += Load;

    public Condition() {
        Level.LevelStarted += (this as ISaveable).InitSaveable;
        All = (Dictionary<string, bool>) (this as ISaveable).LoadData();
        
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
}


