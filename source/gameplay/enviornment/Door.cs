using Godot;
using System;
using KidoUtils;
using System.Collections.Generic;

// When the level finishes, the door opens

public partial class Level {
    public Action OnLevelCompleted; 
    public static Level currentLevel;

    public Dictionary<string, Door> doors;

}

public partial class Door : Area2D {
    // if there's some sort of condition for the door to open, we of course do not want to open it.
    [Export]
    string nextLevel;

    [Export]
    Vector2 doorOpeningDirection;

    // [Export]
    // DoorLink doorLink;

    [Export]
    NodePath otherDoor;


    //PackedScene nextLevel; 
    [Export]
    AnimatedSprite2D sprite;

    [Export]
    Condition condition;

    [Export]
    bool openOnLevelComplete = true;

    public void OpenOtherDoor() {
        // Player.players[0].Position = 
    }

    public override void _Ready() {
        Level.currentLevel.doors.Add(this.Name, this);

        ErrorUtils.AvoidEmptyCollisionLayers(this);
         
        // doorLink.OnSceneSwitched += OpenOtherDoor;
        
        BodyEntered += OnEnterArea;

        if (condition.achieved) {
            OpenDoor();
        } else {
            condition.OnConditionAchieved += OpenDoor;
        } 
    
        Level.currentLevel.OnLevelCompleted += () => Level.currentLevel.doors[this.Name].OpenDoor();
    }

    bool opened = true;
    public void OpenDoor() {
        opened = true;
        sprite.Play("default");
    }

    private void OnEnterArea(Node2D body) {
        if (!opened) return;
        
        if (body is Player player) {
            SceneSwitcher sceneSwitcher = KidoUtils.Utils.GetPreloadedScene<SceneSwitcher>(body, PreloadedScene.SceneSwitcher); 
            sceneSwitcher.ChangeSceneWithPath(nextLevel);
            sceneSwitcher.OnSceneSwitched += () => GD.Print (GetNode(otherDoor));
        }
    }

	public override void _Process(double delta) {
        // this is so dumb but im struggling to solve this somehow.
        if (sprite.Frame + 1 == sprite.SpriteFrames.GetFrameCount("default")) sprite.Pause();
	}
}
