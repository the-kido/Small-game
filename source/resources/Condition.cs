using Godot;
using System;

public static class GameData {
    private static Godot.Collections.Dictionary<string, Variant> SerializeData() =>
        new() {
            {"Conditions", Condition.All},
            {"LevelCompletions", Level.LevelCompletions},
        };

    public static void Save() {
        using FileAccess saveGame = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Write);

        // Serialize the data
        var data = SerializeData();

        // Throw that into the save file
        var stringifiedData = Json.Stringify(data);
        saveGame.StoreLine(stringifiedData);
    }    

    public static Godot.Collections.Dictionary<string, Variant> GetData() {
        if (!FileAccess.FileExists("user://savegame.json")) {
            Save();
            return GetData();
        }

        string data = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Read).GetLine();
 
        Json json = new();

        if (json.Parse(data) != Error.Ok) 
            GD.PushError($"JSON Parse Error: {json.GetErrorMessage()} in {data} at line {json.GetErrorLine()}");

        return new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary) json.Data);
    }
}


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


