using Godot;
using System;
using System.Collections.Generic;

public partial class Condition : Resource {

    public bool achieved = false;
    public Action OnConditionAchieved;
    
    public Condition() {
        All.Add(this,)

        OnConditionAchieved += () => {
            achieved = true;
            Save(); 
            
        };
    }
     

    public void LoadCondition() {
        achieved = Load(); 
    }

    private bool Load() {
        if (!FileAccess.FileExists("user://savegame.save")) {
            GD.Print("Oh no there's no file!");
            return false; // Error! We don't have a save to load.
        }

        using FileAccess saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
        string data = saveGame.GetLine();

        if (data == "true") return true;
        else return false;
    }
    
    private void Save() {
        using FileAccess saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
        var jsonString = Json.Stringify(achieved);
        saveGame.StoreLine(jsonString);
    }   

    static readonly Dictionary<string, bool> All = new();
}


public static class GameDataSaver {
    
}