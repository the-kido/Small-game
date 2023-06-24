using Godot;
using System;

public partial class Condition : Resource {

    public bool achieved = false;
    public Action OnConditionAchieved;
    
    public Condition() {

        OnConditionAchieved += () => {
            achieved = true;
            a(); 
        };
    }

    public void wakeup() {
        GD.Print("wa");
        achieved = GetBoolFromData(); 
    }

    private bool GetBoolFromData() {
        GD.Print("boo");

        using FileAccess saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
        string data = saveGame.GetLine();
        GD.Print("what");
        GD.Print(data);


        if (data == "true") return true;
        else return false;
    }
    
    private void a() {
        using FileAccess saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
        var jsonString = Json.Stringify(achieved);
        saveGame.StoreLine(jsonString);
    }   
}


public static class GameDataSaver {
}