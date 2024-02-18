using Game.LevelContent;
using Game.UI;
using Godot;
using Godot.Collections;
using System;
using System.Linq;

namespace Game.Data;

public record SaveData (string Key, Variant Value);

public class DataSaver {
    public readonly Func<SaveData> getSaveData;

    public DataSaver(Func<SaveData> getSaveData) {
		this.getSaveData = getSaveData;
		RegisterSavedValue();
    }
	
	virtual public Variant LoadValue() => GameDataService.GetData().TryGetValue(getSaveData().Key, out Variant value) ? value : default;

	virtual protected void RegisterSavedValue() {
		// Remove ISaveable's of the same key in case the old object is now gone (i.e a past scene)
		GameDataService.DynamicallySavedItems.RemoveAll((item) => item.getSaveData().Key == getSaveData().Key);

		// Then add this refreshed one		
		GameDataService.DynamicallySavedItems.Add(this);
	}
}
public class RegionalSaveable : DataSaver {
    public RegionalSaveable(Func<SaveData> getSaveData) : base(getSaveData) {
		// ReviveMenu.DeathAccepted += () => getSaveData().Value = "";
	}
    
	public override Variant LoadValue() => RegionManager.CurrentRegion.savedData[getSaveData().Key];
    
	protected override void RegisterSavedValue() {
		RegionManager.Region region = RegionManager.CurrentRegion;

		// Add key if it is not already there
		if (!region.savedData.ContainsKey(getSaveData().Key)) region.savedData.Add(getSaveData().Key, getSaveData().Value);
    }
}

public static class GameDataService {
	const bool USING_DEBUG = false;
	const string SAVE_FILE = "user://savegame.json";
	const string DEBUG_FILE = "user://copy.json";

	public static readonly System.Collections.Generic.List<DataSaver> DynamicallySavedItems = new();
	static Dictionary<string, Variant> SavedItemsAsDictionary => 
		new(DynamicallySavedItems.ToDictionary(x => x.getSaveData().Key, x => x.getSaveData().Value));

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
