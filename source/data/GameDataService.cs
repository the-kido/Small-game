using System.Transactions;
using Godot;
using Godot.Collections;

public interface ISaveable {
    string SaveKey {get;}
    Variant SaveValue {get;}
    Variant LoadData() {
        GD.Print(SaveKey, " is what i'm tryan retrieve");
        GameDataService.GetData().TryGetValue(SaveKey, out Variant value);
        GD.Print(value, " is what I retrieved");
        return value;
    } 
    void InitSaveable() {
        if (GameDataService.dynamicallySavedItems.ContainsKey(SaveKey)) return;

        GameDataService.dynamicallySavedItems.Add(SaveKey, SaveValue);
    } 
}

/*
There is data that is dynamically saved (dynamicallySavedItems) and data drawn
from a previous save file. 
*/
public static class GameDataService {
    public static Dictionary<string, Variant> dynamicallySavedItems = new();

    // Pull from the previous data, then add the new data that needs to be added.
    private static Dictionary<string, Variant> GetCompiledSaveData() {
        Dictionary<string, Variant> newData = new(dynamicallySavedItems);

        foreach (var oldItem in GetData()) {
            // We don't want to overwrite the new data with old data.
            if (newData.ContainsKey(oldItem.Key)) continue;
            newData.Add(oldItem.Key, oldItem.Value);
        }
        
        return newData;
    }

    private static bool fileExists => FileAccess.FileExists("user://savegame.json");

    public static void Save() {
        using FileAccess saveGame = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Write);

        // Serialize the data. If there is no current file, then just draw from whatever was saved so far in dynamicallySavedItems.
        Dictionary<string, Variant> data = fileExists ? GetCompiledSaveData() : dynamicallySavedItems;

        // Throw that into the save file
        var stringifiedData = Json.Stringify(data);
        saveGame.StoreLine(stringifiedData);
    }    

    public static Dictionary<string, Variant> GetData() {
        if (!fileExists) {
            Save();
            return GetData();
        }

        string data = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Read).GetLine();
        Json json = new();

        if (json.Parse(data) != Error.Ok) 
            GD.PushError($"JSON Parse Error: {json.GetErrorMessage()} in {data} at line {json.GetErrorLine()}");

        GD.Print((Dictionary) json.Data, " is what happens when I cast it");
        return new((Dictionary) json.Data);
    }
}