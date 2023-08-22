using Godot;
using Game.Data;
using Game.LevelContent;
using System.Reflection.Metadata.Ecma335;

namespace Game.Players;
// This will be preloaded
public partial class PlayerManager : Node2D, ISaveable {
    public SaveData saveData => new("PlayerClass", currentPlayerClassScript.ResourcePath);
    public override void _Ready() {
        GD.Print("Door: ", queuedDoor);
        if (queuedDoor is not null) {
            GD.Print("Opening");
            Level.CurrentLevel.GetLinkedDoor(queuedDoor).SetPlayerAtDoor();
            queuedDoor = null;
            return;
        } 

        PlacePlayer(Position);
        // Instance base player scene
        // Add class 
        // Update scripts and sprites (within the player class)        
    }
    public override void _ExitTree() {
        instancedPlayer = null;
    }

    public static Player instancedPlayer = null;
    public static Script currentPlayerClassScript = PlayerClasses.NormalPlayerScript;

    static PackedScene player = ResourceLoader.Load<PackedScene>("res://assets/player.tscn");
    
    public static void PlacePlayer(Vector2 position) {
        instancedPlayer = player.Instantiate<Player>();
        
        Level.CurrentLevel.AddChild(instancedPlayer);
        
        instancedPlayer.GlobalPosition = position;
        
        instancedPlayer.SetScript(currentPlayerClassScript);
    }
    
    // Used by the PlayerClassMenu
    public static void SwitchClass(Vector2 newPositionm, Script newPlayerClassScript) {
        currentPlayerClassScript = newPlayerClassScript;
        PlacePlayer(newPositionm);
    }

    static string queuedDoor = null;
    // This is carried out in "ready"
    public static void QueueSpawn(string nextDoor) {
        GD.Print("Queuing");
        queuedDoor = nextDoor.ToString();
    }
}

public static class PlayerClasses {
    public static readonly Script NormalPlayerScript = ResourceLoader.Load<Script>("res://source/actors/player/classes/Normal.cs");
}