using System;
using System.Collections.Generic;
using Game.Data;
using Godot;

namespace Game.LevelContent;

public enum Regions : uint { Dungeon = 0, Nature = 1, Tech = 2, Ice = 3, Center = 4 }

public static class RegionManager {
	public static Regions CurrentRegion {get; private set;} = Regions.Dungeon;
	public static Godot.Collections.Array<bool> RegionsWon {get; private set;}
	
	public const string CENTER_REGION_PATH = "res://assets/levels/center_chamber.tscn";

	// Default to dungeon scene if we are inside the center
	public static Region GetRegionClass(Regions regions) => RegionClasses[regions != Regions.Center ? regions : Regions.Dungeon];

	static readonly Dictionary<Regions, Region> RegionClasses = new() {
		{Regions.Dungeon, new Region(Regions.Dungeon, "res://assets/levels/region-1/Level 1.tscn")},
		{Regions.Nature, new Region(Regions.Nature, "res://assets/levels/debug/level_1.tscn")},
		{Regions.Tech, new Region(Regions.Tech, "res://assets/levels/debug/level_2.tscn")},
		{Regions.Ice, new Region(Regions.Ice, "res://assets/levels/debug/level_3.tscn")},
		{Regions.Center, new Region(Regions.Center, "res://assets/levels/center_chamber.tscn")} // Here to fix TypeInitializationError
	};
	
	public static void RegionWon() {
		// Will set whatever region we are currently in to true.
		RegionsWon[(int) CurrentRegion] = true;
		GameDataService.Save();
	}

	/// <summary>
	/// Called when the first level of a new region is loaded.
	/// </summary>
	public static event Action<Regions> RegionSwitched; 

	static readonly Action<Regions> levelStartedHandler = (goToRegion) => {
		RegionSwitched?.Invoke(goToRegion);
		Level.LevelStarted -= () => levelStartedHandler(goToRegion); // Detach the handler
	};

	public static void SetRegion(Regions goToRegion) {
		CurrentRegion = goToRegion;

		Level.LevelStarted += () => levelStartedHandler(goToRegion);

		GameDataService.Save(); // Update save file to reflect the change in region.
	}


	public static void ResetCurrentRegionData() {
		Region region = RegionClasses[CurrentRegion];
		
		foreach (var item in region.savedData) {
			region.savedData[item.Key] = Json.ParseString("{}");
		}
	}

	static readonly DataSaver currentRegionSaver = 
		new("CurrentRegion", () => CurrentRegion.ToString(), () => CurrentRegion = Regions.Dungeon);
    static readonly DataSaver regionsWonSaver = 
		new("RegionsWon", () => RegionsWon, () => RegionsWon = new() {false, false, false, false});
    
	static RegionManager() {
		string loadedRegion = (string) currentRegionSaver.LoadValue();
		if (!string.IsNullOrEmpty(loadedRegion)) CurrentRegion = Enum.Parse<Regions>(loadedRegion);
		
		RegionsWon = (Godot.Collections.Array<bool>) regionsWonSaver.LoadValue();
		if (RegionsWon.Count == 0) RegionsWon = new() {false, false, false, false};
	}

    public class Region {
		public PackedScene FirstLevel;
		public event Action OnSaveDeleted; 
		public readonly Godot.Collections.Dictionary<string, Variant> savedData = new();
        readonly DataSaver dataSaver;
		public Region(Regions regions, string firstLevelPath) {
			dataSaver = new(regions.ToString(), () => savedData, () => ResetCurrentRegionData());
			savedData = (Godot.Collections.Dictionary<string, Variant>) dataSaver.LoadValue();
			
			Level.LevelStarted += () => {
				FirstLevel = ResourceLoader.Load<PackedScene>(firstLevelPath);	
			};
		}
    }
}