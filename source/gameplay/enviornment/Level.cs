using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Game.Data;
using Game.LevelContent.Criteria;

namespace Game.LevelContent;

public static class RegionManager {
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
    
    public static readonly DataSaver saveable = new(() => new("CurrentRegion", CurrentRegionName));
    static RegionManager() {
		string loadedRegion = (string) saveable.LoadValue();
		if (!string.IsNullOrEmpty(loadedRegion)) CurrentRegionName = loadedRegion;
	}
}

[GlobalClass]
public partial class Level : Node {
	public static event Action LevelStarted;
	public static event Action<LevelCriteria> CriterionStarted;

	public static Level CurrentLevel {get; private set;} = new();
	public static Godot.Collections.Dictionary<string,bool> LevelCompletions {get; private set;} = new();
	public static LevelCriteria CurrentCriterion {get; private set;}

	// The parent is the root of the level, so that's the name we want to save.
	public string SaveName => GetParent().Name;
	public static string CurrentScenePath => CurrentLevel.GetParent().SceneFilePath;

	[Export]
	public Godot.Collections.Array<NodePath> doors = new();
	public event Action LevelCompleted;

	private List<LevelCriteria> levelEvents;

	// Remembers the last level the player was in.
	public static readonly DataSaver lastLevelPlayedSaver = new( () => new("LastLevel", LastLevelFilePath));
	public static string LastLevelFilePath {get; private set;}
	
	public Door GetLinkedDoor(string name) {
		
		foreach (NodePath doorPath in doors) {
			Door door = GetNode<Door>(doorPath);

			if (door.Name == name) 
				return door;
		}
		GD.PushWarning("No such door ", name, " was found for level ", Name);
		return null;
	}

    readonly RegionalSaveable regionalSaveable = new(() => new("LevelCompletions", LevelCompletions));
	public override void _Ready() {
		levelEvents = GetChildren().Cast<LevelCriteria>().ToList();
		
		LevelCompleted += GameDataService.Save;
		
		Change();

		if (!LoadCompletion())
			CompleteAllEvents(0);
		else
			Complete();
	}

	// I ♥ recursion
	private void CompleteAllEvents(int index) {
		if (index == levelEvents.Count) {
			Complete();
			return;
		}
		
		CurrentCriterion = levelEvents[index];
		
		LevelCriteria currentCriterion = levelEvents[index];

		currentCriterion.Finished += () => CompleteAllEvents(index + 1);
		currentCriterion.CallDeferred("Start");
		
		// Because the above is a deferred call, I have to invoke CriterionStarted deferred too; we will have a race condition otherwise
		CallDeferred("InvokeCriterionStarted", currentCriterion);
	}

	private void InvokeCriterionStarted(LevelCriteria currentCriterion) =>
		CriterionStarted?.Invoke(currentCriterion);

	private void Change() {
		CurrentLevel = this;
		LastLevelFilePath = CurrentLevel.GetParent().SceneFilePath; 
		LevelStarted?.Invoke();
	}

	private void Complete() {
		CurrentCriterion = null;
		LevelCompletions[SaveName] = true;
		LevelCompleted?.Invoke();
	}

	private bool LoadCompletion() {
		LevelCompletions = (Godot.Collections.Dictionary<string, bool>) regionalSaveable.LoadValue();
		return LevelCompletions.ContainsKey(SaveName) && LevelCompletions[SaveName];
	}

	public static bool IsCurrentLevelCompleted() {
		bool valueRecieved = LevelCompletions.TryGetValue(CurrentLevel.SaveName, out bool levelWon);
		return valueRecieved && levelWon;
	}

	/// <summary>
	/// Is required for static mechanics which are relative to the scope of a level.
	/// I.E Interactables are contained within a level and need to update per frame the level is awake
	/// </summary>
	public static event Action LevelProcessedFrame;
	public override void _Process(double delta) => LevelProcessedFrame?.Invoke(); 
}

