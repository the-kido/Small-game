using Godot;
using System;
using KidoUtils;

// When the level finishes, the door opens

public partial class Level {
    public Action OnLevelCompleted; 
    public static Level currentLevel;

}

public partial class Door : Area2D {
    // if there's some sort of condition for the door to open, we of course do not want to open it.
    [Export]
    DoorLink doorLink;

    [Export]
    string nextLevel;

    //PackedScene nextLevel; 
    [Export]
    AnimatedSprite2D sprite;

    [Export]
    Condition condition;

    [Export]
    bool openOnLevelComplete = true;

    public override void _Ready() {
        
        ErrorUtils.AvoidEmptyCollisionLayers(this);
        
        BodyEntered += OnEnterArea;

        if (condition.achieved) {
            OpenDoor();
        } else {
            condition.OnConditionAchieved += OpenDoor;
        } 

        //Level.currentLevel.OnLevelCompleted += OpenDoor;
    }

    bool opened = true;
    public void OpenDoor() {
        opened = true;
        sprite.Play("default");
    }

    private void OnEnterArea(Node2D body) {
        if (!opened) return;
        
        if (body is Player player) {
            GD.Print(doorLink.direction1);
            GD.Print(doorLink.firstDoor);
            GD.Print(doorLink.secondDoor);

            KidoUtils.Utils.GetPreloadedScene<SceneSwitcher>(body, PreloadedScene.SceneSwitcher).ChangeSceneWithPath(nextLevel);
        }
    }

	public override void _Process(double delta) {
        // this is so dumb but im struggling to solve this somehow.
        if (sprite.Frame + 1 == sprite.SpriteFrames.GetFrameCount("default")) sprite.Pause();
	}
}
