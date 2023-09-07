using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Game.Data;
using Game.LevelContent.Criteria;

namespace Game.LevelContent;

[GlobalClass]
public partial class Level : Node, ISaveable {

    public static event Action LevelStarted;
    public static event Action<LevelCriteria> CriterionStarted;

    public static Level CurrentLevel {get; private set;} = new();
    public static Godot.Collections.Dictionary<string,bool> LevelCompletions {get; private set;} = new();
    public static LevelCriteria CurrentEvent {get; private set;}

    public SaveData SaveData => new("LevelCompletions", LevelCompletions);
    // The parent is the root of the level, so that's the name we want to save.
    public string SaveName => GetParent().Name;

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
        return null;
    }

    public override void _Ready() {
        (this as ISaveable).InitSaveable();
        
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
        
        CurrentEvent = levelEvents[index];
        
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
        CurrentEvent = null;
        LevelCompletions[SaveName] = true;
        LevelCompleted?.Invoke();
    }

    private bool LoadCompletion() {
        LevelCompletions = (Godot.Collections.Dictionary<string, bool>) (this as ISaveable).LoadData();
        return LevelCompletions.ContainsKey(SaveName) && LevelCompletions[SaveName];
    }

    public static bool IsCurrentLevelCompleted() {
        bool valueRecieved = LevelCompletions.TryGetValue(CurrentLevel.SaveName, out bool levelWon);
        return valueRecieved && levelWon;
    }
}

public class LastLevelPlayedSaver : ISaveable {
    public SaveData SaveData => new("LastLevel", Level.LastLevelFilePath);
    public LastLevelPlayedSaver() => (this as ISaveable).InitSaveable();
}