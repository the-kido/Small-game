using Game.LevelContent;
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
	
	virtual public Variant LoadValue() => GameDataService.fileWritter.GetData().TryGetValue(getSaveData().Key, out Variant value) ? value : default;

	virtual protected void RegisterSavedValue() {
		// Remove ISaveable's of the same key in case the old object is now gone (i.e a past scene)
		GameDataService.fileWritter.DynamicallySavedItems.RemoveAll((item) => item.getSaveData().Key == getSaveData().Key);

		// Then add this refreshed one		
		GameDataService.fileWritter.DynamicallySavedItems.Add(this);
	}
}
public class RegionalSaveable : DataSaver {
    public RegionalSaveable(Func<SaveData> getSaveData) : base(getSaveData) {}
    
	public override Variant LoadValue() => RegionManager.GetRegionClass(RegionManager.CurrentRegion).savedData[getSaveData().Key];

	protected override void RegisterSavedValue() {
		RegionManager.Region region = RegionManager.GetRegionClass(RegionManager.CurrentRegion);

		// Add key if it is not already there
		if (!region.savedData.ContainsKey(getSaveData().Key)) region.savedData.Add(getSaveData().Key, getSaveData().Value);
    }
}

public class UserDataSaver : DataSaver {
    public UserDataSaver(Func<SaveData> getSaveData) : base(getSaveData) {}

    public override Variant LoadValue() => UserDataService.fileWritter.GetData().TryGetValue(getSaveData().Key, out Variant value) ? value : default;

    protected override void RegisterSavedValue() {
    	UserDataService.fileWritter.DynamicallySavedItems.RemoveAll((item) => item.getSaveData().Key == getSaveData().Key);
		UserDataService.fileWritter.DynamicallySavedItems.Add(this);
	}
}
// (?)
public class FileWritter {
	readonly string saveFilePath;
	readonly string readFilePath;

	public FileWritter(string saveFilePath, string readFilePath = "") {
		this.readFilePath = string.IsNullOrEmpty(readFilePath) ? saveFilePath : readFilePath;
        this.saveFilePath = saveFilePath;
	}

	public readonly System.Collections.Generic.List<DataSaver> DynamicallySavedItems = new();
	private Dictionary<string, Variant> SavedItemsAsDictionary => 
		new(DynamicallySavedItems.ToDictionary(x => x.getSaveData().Key, x => x.getSaveData().Value));

	// Pull from the previous data, then add the new data that needs to be added.
	private Dictionary<string, Variant> GetCompiledSaveData() {
		Dictionary<string, Variant> newData = SavedItemsAsDictionary;

		foreach (var oldItem in GetData()) {
			// We don't want to overwrite the new data with old data.
			if (newData.ContainsKey(oldItem.Key))
				continue;

			newData.Add(oldItem.Key, oldItem.Value);
		}

		return newData;
	}

	private bool FileExists => 
		FileAccess.FileExists(saveFilePath);

	public void Save() {
		// Serialize the data. If there is no current file, then just draw from whatever was saved so far in dynamicallySavedItems.
		Dictionary<string, Variant> data = FileExists ? GetCompiledSaveData() : SavedItemsAsDictionary;

		using FileAccess saveFile = FileAccess.Open(saveFilePath, FileAccess.ModeFlags.Write);

		// Throw that into the save file
		var stringifiedData = Json.Stringify(data);
		saveFile.StoreLine(stringifiedData);
	}    

	public Dictionary<string, Variant> GetData() {
		
		if (!FileExists) {
			Save();
			return GetData();
		}

		using FileAccess saveFile = FileAccess.Open(readFilePath, FileAccess.ModeFlags.Read);
		string data = saveFile.GetLine();

		if (string.IsNullOrEmpty(data)) 
			return new();
		
		Json json = new();

		if (json.Parse(data) != Error.Ok) 
			GD.PushError($"JSON Parse Error: {json.GetErrorMessage()} in {data} at line {json.GetErrorLine()}");

		return new((Dictionary) json.Data);
	}
}


public static class GameDataService {
	const bool USING_DEBUG = false;
	const string SAVE_FILE = "user://savegame.json";
	const string DEBUG_FILE = "user://copy.json";

	internal static readonly FileWritter fileWritter = new(SAVE_FILE, USING_DEBUG ? DEBUG_FILE : "");

	public static void Save() => fileWritter.Save();
}

public static class UserDataService {
	const string SAVE_FILE = "user://playerdata.json";

	internal static readonly FileWritter fileWritter = new(SAVE_FILE);

	public static void Save() => fileWritter.Save();
}