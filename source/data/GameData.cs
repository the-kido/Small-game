using System;
using Godot;
using Godot.Collections;

public static class GameData {
    private static Dictionary<string, Variant> SerializeData() =>
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

    public static Dictionary<string, Variant> GetData() {
        if (!FileAccess.FileExists("user://savegame.json")) {
            Save();
            return GetData();
        }

        string data = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Read).GetLine();
 
        Json json = new();

        if (json.Parse(data) != Error.Ok) 
            GD.PushError($"JSON Parse Error: {json.GetErrorMessage()} in {data} at line {json.GetErrorLine()}");

        return new Dictionary<string, Variant>((Dictionary) json.Data);
    }
}