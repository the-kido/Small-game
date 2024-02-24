using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Game.Data;
using Game.LevelContent.Criteria;

namespace Game.LevelContent;

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

	private List<LevelCriteria> levelCriterion;

	// Remembers the last level the player was in.
	public static string LastLevelFilePath {get; private set;}
	public static readonly DataSaver lastLevelPlayedSaver = 
		new("LastLevel", () => LastLevelFilePath, () => LastLevelFilePath = FIRST_LEVEL_PATH);
	
	public LevelSwitchRegion GetLinkedDoor(string name) {
		
		foreach (NodePath doorPath in doors) {
			LevelSwitchRegion door = GetNode<LevelSwitchRegion>(doorPath);

			if (door.Name == name) 
				return door;
		}
		return null;
	}

    static readonly RegionDataSaver regionalSaveable = 
		new("LevelCompletions", () => LevelCompletions, () => LevelCompletions = new());
	
	public override void _Ready() {
		levelCriterion = GetLevelCriterion();

        LevelCompleted += GameDataService.Save;
		
		Change();

		if (!LoadCompletion()) CompleteAllEvents(0, levelCriterion);
		else Complete();
	}
	private List<LevelCriteria> GetLevelCriterion() => GetChildren().Cast<LevelCriteria>().ToList();
	
	// I ♥ recursion
	public void CompleteAllEvents(int index, List<LevelCriteria> criterion) {
		if (index == criterion.Count) {
			Complete();
			return;
		}
		
		CurrentCriterion = criterion[index];
		
		LevelCriteria currentCriterion = criterion[index];

		currentCriterion.Finished += () => CompleteAllEvents(index + 1, criterion);
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

	public static readonly string FIRST_LEVEL_PATH = "res://assets/levels/debug/spawn.tscn";
}

