using Godot;
using Game.LevelContent;
using System.Collections.Generic;
using Game.Actors;
using Game.SealedContent;

namespace Game.Players;

public partial class PlayerManager : Node2D {

    public override void _Ready() {
        instancedPlayer = null;

        PlacePlayer(GetSpawnPosition());

        if (queuedDoor is not null) 
            queuedDoor = null;
    }

    private Vector2 GetSpawnPosition() => 
        queuedDoor is not null ? Level.CurrentLevel.GetLinkedDoor(queuedDoor).PlayerSpawnPosition - Position : Vector2.Zero;

    static readonly PackedScene PlayerScene = ResourceLoader.Load<PackedScene>("res://assets/player.tscn");
    
    static Player instancedPlayer = null;

    private void PlacePlayer(Vector2 position) {

        if (instancedPlayer is not null) 
            return;
        
        instancedPlayer = PlayerScene.Instantiate<Player>();

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
    public static readonly Normal normal = new();

    public static Dictionary<string, PlayerClassResource> List => new() {
        {"Normal", ResourceLoader.Load<PlayerClassResource>("res://assets/player/classes/default.tres")},
    };

    // Required for "PlayerClassMenu", which is currently unused.
    public static Dictionary<string, IPlayerClass> Other => new() {
        {"Normal", normal},
    };
}