using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Level : Node {

    public static event Action LevelStarted;
    public static Level CurrentLevel {get; private set;} = new();
    public static Godot.Collections.Dictionary<string,bool> LevelCompletions {get; private set;} = new();
    
    [Export]
    public Godot.Collections.Array<NodePath> doors = new();
    public event Action LevelCompleted;

    private List<LevelEvent> levelEvents;

    public Door GetLinkedDoor(string name) {
        foreach (NodePath doorPath in doors) {
            Door door = GetNode<Door>(doorPath);
            if (door.Name == name) return door;
        }
        return null;
    }

    public override void _Ready() {
        levelEvents = GetChildren().Cast<LevelEvent>().ToList();
        LevelCompleted += GameData.Save;
        
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
        levelEvents[index].Start();
        levelEvents[index].Finished += () => CompleteAllEvents(index + 1);
    }

    private void Change() {
        CurrentLevel = this;
        LevelStarted?.Invoke();
    }
    
    private void Complete() {
        LevelCompletions[Name] = true;
        LevelCompleted?.Invoke();
    }
    
    private bool LoadCompletion() {
        LevelCompletions = (Godot.Collections.Dictionary<string,bool>) GameData.GetData()["LevelCompletions"];
        
        if (LevelCompletions.ContainsKey(Name))
            return LevelCompletions[Name];
        else
            return false;
    }
}

public partial class Level : Node {

    readonly FreezeOrbMechanic freezeOrbMechanic = new();
    public override void _Process(double delta) {
        freezeOrbMechanic.Process(delta);
    }
}

public class FreezeOrbMechanic {

    const int FreezeTime = 5;
    float freezeTimer = FreezeTime;

    static bool isFrozen = false;
    public FreezeOrbMechanic() {
        DungeonRunData.FreezeWave += () => {
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