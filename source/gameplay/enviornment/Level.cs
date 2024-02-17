using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Game.Data;
using Game.LevelContent.Criteria;

namespace Game.LevelContent;

public class RegionManager : ISaveable {
    public SaveData SaveData => new("CurrentRegion", CurrentRegionName.ToString());

	public string CurrentRegionName {get; private set;} = Regions[0];
	public Region CurrentRegion => RegionClasses[CurrentRegionName];
	public static readonly string[] Regions = new string[] {
		"DungeonRegion",
		"NatureRegion",
		"TechRegion",
		"IceRegion"
	};
	public static readonly Dictionary<string, Region> RegionClasses = new() {
		{Regions[0], new Dungeon()},
		{Regions[1], new Nature()},
		{Regions[2], new Tech()},
		{Regions[3], new Ice()}
	};
	
	public static void ResetRegionData(Region region) {
		region.savedData = new();
		GameDataService.Save();
	}

    // I could have a bit of data for every region, and delete it depenidn go
    public abstract class Region {
		public Godot.Collections.Dictionary<string, Variant> savedData = new();
    }
    public class Dungeon : Region, ISaveable {
		public Dungeon() {
			(this as ISaveable).InitSaveable();
			savedData = (Godot.Collections.Dictionary<string, Variant>) (this as ISaveable).LoadData();
		}

		public SaveData SaveData => new(Regions[0], savedData);
    }
    public class Nature : Region, ISaveable {
        public Nature() : base() {}
		public SaveData SaveData => new(Regions[1], savedData);
    }
    public class Tech : Region, ISaveable {
        public Tech() => (this as ISaveable).InitSaveable();
		public SaveData SaveData => new(Regions[2], savedData);
    }
    public class Ice : Region, ISaveable {
        public Ice() => (this as ISaveable).InitSaveable();
		public SaveData SaveData => new(Regions[3], savedData);
    }

    public RegionManager() {
		(this as ISaveable).InitSaveable();
		string loadedRegion = (string) (this as ISaveable).LoadData();
		if (!string.IsNullOrEmpty(loadedRegion)) CurrentRegionName = loadedRegion;
	}
}

[GlobalClass]
public partial class Level : Node, IRegionalSaveable {

	public static event Action LevelStarted;
	public static event Action<LevelCriteria> CriterionStarted;

	public static Level CurrentLevel {get; private set;} = new();
	public static RegionManager RegionManager {get; private set;} = new();
	public static Godot.Collections.Dictionary<string,bool> LevelCompletions {get; private set;} = new();
	public static LevelCriteria CurrentCriterion {get; private set;}

	public SaveData SaveData => new("LevelCompletions", LevelCompletions);
	// The parent is the root of the level, so that's the name we want to save.
	public string SaveName => GetParent().Name;
	public static string CurrentScenePath => CurrentLevel.GetParent().SceneFilePath;

	[Export]
	public Godot.Collections.Array<NodePath> doors = new();
	public event Action LevelCompleted;

	private List<LevelCriteria> levelEvents;

	public static LastLevelPlayedSaver LastLevelPlayedSaver {get; private set;}
	public static string LastLevelFilePath {get; private set;}
	static Level() => LastLevelPlayedSaver = new();

	public Door GetLinkedDoor(string name) {
		
		foreach (NodePath doorPath in doors) {
			Door door = GetNode<Door>(doorPath);

			if (door.Name == name) 
				return door;
		}
		GD.PushWarning("No such door ", name, " was found for level ", Name);
		return null;
	}

	public override void _Ready() {
		(this as IRegionalSaveable).InitRegionSaveable();
		
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
		LevelCompletions = (Godot.Collections.Dictionary<string, bool>) (this as IRegionalSaveable).LoadData();
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

public class LastLevelPlayedSaver : ISaveable {
	public SaveData SaveData => new("LastLevel", Level.LastLevelFilePath);
	public LastLevelPlayedSaver() => (this as ISaveable).InitSaveable();
}
