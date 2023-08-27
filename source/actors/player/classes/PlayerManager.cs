using Godot;
using Game.Data;
using Game.LevelContent;
using System.Collections.Generic;
using Game.Actors;
using System;

namespace Game.Players;
// This will be preloaded
public partial class PlayerManager : Node2D, ISaveable {
    public SaveData SaveData => new("PlayerClass", currentPlayerClassScript.ResourcePath);
    
    private Script LoadClass() {
        string pathToScript = (string) (this as ISaveable).LoadData();
        return string.IsNullOrEmpty(pathToScript) ? PlayerClasses.NormalPlayerScript : PlayerClasses.temp[pathToScript];
    }
    
    public override void _Ready() {
        (this as ISaveable).InitSaveable();
        
        currentPlayerClassScript = LoadClass();
        
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

        instancedPlayer.SetScript(currentPlayerClassScript);

        InitNewPlayerScript(serializedData); 
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
    public static event Action ClassSwitched;
    public static void SwitchClass(Script newPlayerClassScript, Vector2 position) {
        currentPlayerClassScript = newPlayerClassScript;
        
        foreach (Node child in Level.CurrentLevel.GetChildren()) {
            if (child is Player player)
                Level.CurrentLevel.RemoveChild(player);
        }
        ClassSwitched?.Invoke();
        
        PlacePlayer(position);
        
        GameDataService.Save(); // Save the change in class
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

    public static Dictionary<Script, PlayerClassResource> List => new() {
        {NormalPlayerScript, ResourceLoader.Load<PlayerClassResource>("res://assets/content/classes/default.tres")},
        {WeirdPlayerScript, ResourceLoader.Load<PlayerClassResource>("res://assets/content/classes/weird.tres")},
    };

    public static Godot.Collections.Dictionary<string, Script> temp => new() {
        {NormalPlayerScript.ResourcePath, NormalPlayerScript},
        {WeirdPlayerScript.ResourcePath, WeirdPlayerScript}
    };
}