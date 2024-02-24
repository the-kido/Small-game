using Godot;
using Godot.Collections;
using System;
using System.ComponentModel;
using Game.LevelContent;

namespace Game.Data;

[GlobalClass]
public partial class Condition : Resource {
    
    [Export, Description("Required to differentiate condition entries in the save file")]
    private string Name; 

    public Action Achieved;
    public bool IsAchieved {get; private set;} = false;
    
    // Required for saving the conditions
    public static Dictionary<string, bool> All {get; private set;} = new();
    readonly static DataSaver dataSaver = 
        new("Conditions",  () => All, () => All = new());

    private static void Load() => All = (Dictionary<string,bool>) dataSaver.LoadValue();
    private bool GetAchieved() => All.ContainsKey(Name) && All[Name];
    
    static Condition() => Level.LevelStarted += Load;

    public Condition() {
        All = (Dictionary<string, bool>) dataSaver.LoadValue();
    }
    
    public void Achieve() {
        IsAchieved = true;
        All[Name] = true;
        Achieved?.Invoke();
    }

    public void Init() {
        if (string.IsNullOrEmpty(Name)) 
            throw new ArgumentNullException("Name for condition must be set!");
        
        IsAchieved = GetAchieved(); 
    }
}


