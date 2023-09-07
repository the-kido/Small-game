using Godot;
using Game.LevelContent;
using System.Collections.Generic;
using Game.Actors;
using Game.SealedContent;

namespace Game.Players;

[GlobalClass]
public partial class PlayerManager : Node2D {
    static PlayerManager currentPlayerManager;
    public override void _Ready() {
        
        currentPlayerManager = this;
        instancedPlayer = null;
        
        Vector2 spawnPosition;

        if (queuedDoor is not null) {
            spawnPosition = Level.CurrentLevel.GetLinkedDoor(queuedDoor).PlayerSpawnPosition - Position;
            queuedDoor = null;
        } else {
            spawnPosition = Vector2.Zero;
        }

        PlacePlayer(spawnPosition);
    }

    static readonly PackedScene playerScene = ResourceLoader.Load<PackedScene>("res://assets/player.tscn");
    
    static Player instancedPlayer = null;

    private void PlacePlayer(Vector2 position) {

        if (instancedPlayer is not null) 
            return;
        
        instancedPlayer = playerScene.Instantiate<Player>();

        instancedPlayer.Position = position;

        AddChild(instancedPlayer);
        
        instancedPlayer.Init();
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
    public static Dictionary<string, IPlayerClass> Other => new() {
        {"Normal", normal},
        {"Weird", weird},
    };
}