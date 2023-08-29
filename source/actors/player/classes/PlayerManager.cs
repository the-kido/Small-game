using Godot;
using Game.Data;
using Game.LevelContent;
using System.Collections.Generic;
using Game.Actors;
using System;
using Game.SealedContent;

namespace Game.Players;

public partial class PlayerManager : Node2D {

    public override void _Ready() {
        
        instancedPlayer = null;
        
        if (queuedDoor is not null) {
            Level.CurrentLevel.GetLinkedDoor(queuedDoor).SetPlayerAtDoor();
            queuedDoor = null;
            return;
        } 

        PlacePlayer(Position);
    }

    static readonly PackedScene playerScene = ResourceLoader.Load<PackedScene>("res://assets/player.tscn");
    
    static Player instancedPlayer = null;

    public static void PlacePlayer(Vector2 position) {
        if (instancedPlayer is null) {
            instancedPlayer = playerScene.Instantiate<Player>();

            instancedPlayer.GlobalPosition = position;
            
            Level.CurrentLevel.AddChild(instancedPlayer);
            
            instancedPlayer.Init();
        } 
    }
    
    /// <summary>
    /// Invoked after the player class is switched. It's invocation list is cleared when the game scene changes.
    /// </summary>
    public static event Action<PlayerClass> ClassSwitched;

    PlayerManager() =>
        ClassSwitched = null; // Clear all subscribers

    public static void SwitchClass(PlayerClass newPlayerClass) {
        instancedPlayer.SetClass(newPlayerClass);
        ClassSwitched?.Invoke(newPlayerClass);
        
        GameDataService.Save(); // Save the change in class
    }

    static string queuedDoor = null;
    // This is carried out in "ready"
    public static void QueueSpawn(string nextDoor) {
        queuedDoor = nextDoor.ToString();
    }
}

public static class PlayerClasses {
    public static readonly Weird weird = new();
    public static readonly Normal normal = new();
    public static readonly Script NormalPlayerScript = ResourceLoader.Load<Script>("res://source/actors/player/classes/Normal.cs");
    public static readonly Script WeirdPlayerScript = ResourceLoader.Load<Script>("res://source/actors/player/classes/Weird.cs");

    public static Dictionary<string, PlayerClassResource> List => new() {
        {"Normal", ResourceLoader.Load<PlayerClassResource>("res://assets/content/classes/default.tres")},
        {"Weird", ResourceLoader.Load<PlayerClassResource>("res://assets/content/classes/weird.tres")},
    };
    public static Dictionary<string, PlayerClass> Other => new() {
        {"Normal", normal},
        {"Weird", weird},
    };
}