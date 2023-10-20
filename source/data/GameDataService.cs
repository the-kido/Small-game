using Godot;
using Godot.Collections;
using System.Linq;

namespace Game.Data;

public record SaveData (string Key, Variant Value);

public interface ISaveable {
	SaveData SaveData {get;}
	
	Variant LoadData() {
		GameDataService.GetData().TryGetValue(SaveData.Key, out Variant value);
		return value;
	}
	
	void InitSaveable() {
		// Make sure there's no duplicates
		if (GameDataService.dynamicallySavedItems.Any(item => item.SaveData.Key == SaveData.Key)) 
			return;
		
		GameDataService.dynamicallySavedItems.Add(this);
	}
}

/*
There is data that is dynamically saved (dynamicallySavedItems) and data drawn
from a previous save file. 
*/
public static class GameDataService {

	const bool USING_DEBUG = true;
	const string SAVE_FILE = "user://savegame.json";
	const string DEBUG_FILE = "user://copy.json";

	public static readonly System.Collections.Generic.List<ISaveable> dynamicallySavedItems = new();
	static Dictionary<string, Variant> SavedItemsAsDictionary => 
		new(dynamicallySavedItems.ToDictionary(x => x.SaveData.Key, x => x.SaveData.Value));

	// Pull from the previous data, then add the new data that needs to be added.
	private static Dictionary<string, Variant> GetCompiledSaveData() {
		Dictionary<string, Variant> newData = SavedItemsAsDictionary;

		foreach (var oldItem in GetData()) {
			// We don't want to overwrite the new data with old data.
			if (newData.ContainsKey(oldItem.Key)) 
				continue;
			
			newData.Add(oldItem.Key, oldItem.Value);
		}
		
		return newData;
	}

	private static bool FileExists => 
		FileAccess.FileExists(SAVE_FILE);

	public static void Save() {
		
		// Serialize the data. If there is no current file, then just draw from whatever was saved so far in dynamicallySavedItems.
		Dictionary<string, Variant> data = FileExists ? GetCompiledSaveData() : SavedItemsAsDictionary;

		using FileAccess saveFile = FileAccess.Open(SAVE_FILE, FileAccess.ModeFlags.Write);

		// Throw that into the save file
		var stringifiedData = Json.Stringify(data);
		saveFile.StoreLine(stringifiedData);
	}    

	public static Dictionary<string, Variant> GetData() {
		
		if (!FileExists) {
			Save();
			return GetData();
		}

		using FileAccess saveFile = FileAccess.Open(USING_DEBUG ? DEBUG_FILE : SAVE_FILE, FileAccess.ModeFlags.Read);
		string data = saveFile.GetLine();

		if (string.IsNullOrEmpty(data)) 
			return new();
		
		Json json = new();

		if (json.Parse(data) != Error.Ok) 
			GD.PushError($"JSON Parse Error: {json.GetErrorMessage()} in {data} at line {json.GetErrorLine()}");

		return new((Dictionary) json.Data);
	}
}
