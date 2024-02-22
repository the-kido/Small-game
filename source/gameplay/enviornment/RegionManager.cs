using System;
using System.Collections.Generic;
using Game.Data;
using Godot;

namespace Game.LevelContent;

public enum Regions : uint { Dungeon = 0, Nature = 1, Tech = 2, Ice = 3, CenterRegion = 4 }

public static class RegionManager {
	public const string CENTER_REGION_PATH = "res://assets/levels/center_chamber.tscn";
	public static readonly string[] Regions = new string[] {
		"DungeonRegion",
		"NatureRegion",
		"TechRegion",
		"IceRegion"
	};

	public static string CurrentRegionName {get; private set;} = Regions[0];
	public static readonly Dictionary<string, Region> RegionClasses = new() {
		{Regions[0], new Region(Regions[0], "res://assets/levels/region-1/Level 1.tscn")},
		{Regions[1], new Region(Regions[1], "res://assets/levels/debug/level_1.tscn")},
		{Regions[2], new Region(Regions[2], "res://assets/levels/debug/level_2.tscn")},
		{Regions[3], new Region(Regions[3], "res://assets/levels/debug/level_3.tscn")}
	};
	public static Region CurrentRegion => RegionClasses[CurrentRegionName];
	
	public static readonly DataSaver currentRegionSaver = new(() => new("CurrentRegion", CurrentRegionName));
    static RegionManager() {
		string loadedRegion = (string) currentRegionSaver.LoadValue();
		if (!string.IsNullOrEmpty(loadedRegion)) CurrentRegionName = loadedRegion;
		
		RegionsWon = (Godot.Collections.Array<bool>) regionsWonSaver.LoadValue();
		if (RegionsWon.Count == 0) RegionsWon = new() {false, false, false, false};
	}

	public static Godot.Collections.Array<bool> RegionsWon {get; private set;}
    static readonly DataSaver regionsWonSaver = new(() => new("RegionsWon", RegionsWon));
	public static void RegionWon() {
		// Will set whatever region we are currently in to true.
		int i = Array.IndexOf(Regions, CurrentRegionName);
		RegionsWon[i] = true;
		GameDataService.Save();
	}

	public static void SetRegion(string regionName) {
		CurrentRegionName = regionName;
		GameDataService.Save(); // Update save file to reflect the change in region.
	}

	public static void ResetRegionData(Region region) {
		foreach (var item in region.savedData) {
			region.savedData[item.Key] = Json.ParseString("{}");
		}
	}

    // I could have a bit of data for every region, and delete it depenidn go
    public class Region {
		public PackedScene FirstLevel;
		public event Action OnSaveDeleted; 
		public readonly Godot.Collections.Dictionary<string, Variant> savedData = new();
        readonly DataSaver dataSaver;
		public Region(string key, string firstLevelPath) {
			dataSaver = new(() => new(key, savedData));
			savedData = (Godot.Collections.Dictionary<string, Variant>) dataSaver.LoadValue();
			
			Level.LevelStarted += () => {
				FirstLevel = ResourceLoader.Load<PackedScene>(firstLevelPath);	
			};
		}
    }
}