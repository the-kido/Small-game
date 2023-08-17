using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Game.Data;
using Game.LevelContent.Criteria;

namespace Game.LevelContent;

public partial class Level : Node, ISaveable{

    public static event Action LevelStarted;
    public static Level CurrentLevel {get; private set;} = new();
    public static Godot.Collections.Dictionary<string,bool> LevelCompletions {get; private set;} = new();

    public SaveData saveData => new("LevelCompletions", LevelCompletions);
    // The parent is the root of the level, so that's the name we want to save.
    public string SaveName => GetParent().Name;

    [Export]
    public Godot.Collections.Array<NodePath> doors = new();
    public event Action LevelCompleted;

    private List<LevelCriteria> levelEvents;
    public static LevelCriteria CurrentEvent {get; private set;}

    public Door GetLinkedDoor(string name) {
        foreach (NodePath doorPath in doors) {
            Door door = GetNode<Door>(doorPath);
            if (door.Name == name) return door;
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
        levelEvents[index].Finished += () => CompleteAllEvents(index + 1);
        levelEvents[index].CallDeferred("Start");
    }

    private void Change() {
        CurrentLevel = this;
        LevelStarted?.Invoke();
    }    
    private void Complete() {
        CurrentEvent = null;
        LevelCompletions[SaveName] = true;
        LevelCompleted?.Invoke();
    }
    private bool LoadCompletion() {
        LevelCompletions = (Godot.Collections.Dictionary<string,bool>) (this as ISaveable).LoadData();
        return LevelCompletions.ContainsKey(SaveName) ? LevelCompletions[SaveName] : false;
    }
}

public class FreezeOrbMechanic {

    const int FreezeTime = 5;
    float freezeTimer = FreezeTime;

    static bool isFrozen = false;
    public FreezeOrbMechanic() {
        DungeonRunData.FreezeOrbs.FreezeWave += () => {
            Freeze(true);
            freezeTimer = 0;
        }; 
    }

    public void Process(double delta) {
        if (freezeTimer >= FreezeTime) {
            if (isFrozen)
                Freeze(false);
            return;
        }
        
        freezeTimer += (float) delta;
    }

    private static void Freeze(bool freeze) {
        isFrozen = freeze;

        /* 
        if (Level.Temp()) return;
        
        Node.ProcessModeEnum processMode = freeze ? Node.ProcessModeEnum.Disabled : Node.ProcessModeEnum.Always;
        // Replace water effect with ice effect obv
        Level.CurrentWave?.EnemyChildren.ForEach(child => child.DamageableComponent.Damage(new(child) {statusEffect = new WetStatus()}  ));
        // TODO: Replace this with making the shooting speed multiplier 0 and making its speed multiplier 0 too.
        // that way things like efffects will still apply to it.
        Level.CurrentWave.ProcessMode = processMode;
        */
    }
}