using Godot;
using Game.Data;
using Game.LevelContent;
using System.Collections.Generic;

namespace Game.Players;
// This will be preloaded
public partial class PlayerManager : Node2D, ISaveable {
    public SaveData saveData => new("PlayerClass", currentPlayerClassScript.ResourcePath);
    public override void _Ready() {
        if (queuedDoor is not null) {
            Level.CurrentLevel.GetLinkedDoor(queuedDoor).SetPlayerAtDoor();
            queuedDoor = null;
            return;
        } 

        PlacePlayer(Position);
    }

    static Script currentPlayerClassScript = PlayerClasses.WeirdPlayerScript;

    static readonly PackedScene playerScene = ResourceLoader.Load<PackedScene>("res://assets/player.tscn");
    
    public static void PlacePlayer(Vector2 position) {
        Player instancedPlayer = playerScene.Instantiate<Player>();
        
        Dictionary<string, Variant> serializedData = instancedPlayer.SerializedPlayerExports;

        instancedPlayer.GlobalPosition = position;
        
        Level.CurrentLevel.AddChild(instancedPlayer);

        instancedPlayer.ScriptChanged += () => InitNewPlayerScript(serializedData); 
        instancedPlayer.SetScript(currentPlayerClassScript);
    }
    private static void InitNewPlayerScript(Dictionary<string, Variant> serializedData) {
        foreach (Node child in Level.CurrentLevel.GetChildren()) {
            if (child is Player player) {
                player.SetDataFromSerializedExports(serializedData);
                player.Init();
            }
        }
    }
    
    // Used by the PlayerClassMenu
    public static void SwitchClass(Vector2 newPositionm, Script newPlayerClassScript) {
        currentPlayerClassScript = newPlayerClassScript;
        PlacePlayer(newPositionm);
    }

    static string queuedDoor = null;
    // This is carried out in "ready"
    public static void QueueSpawn(string nextDoor) {
        queuedDoor = nextDoor.ToString();
    }
}

public static class PlayerClasses {
    public static readonly Script NormalPlayerScript = ResourceLoader.Load<Script>("res://source/actors/player/classes/Normal.cs");
    public static readonly Script WeirdPlayerScript = ResourceLoader.Load<Script>("res://source/actors/player/classes/Weird.cs");
}