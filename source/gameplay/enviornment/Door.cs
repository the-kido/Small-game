using Godot;
using KidoUtils;
using Game.Players;
using Game.Data;

namespace Game.LevelContent;

public partial class Door : Area2D {
    
    [Export]
    private LevelSwitcher levelSwitcher;
    [Export]
    private Vector2I doorOpeningDirection;
    [Export]
    private AnimatedSprite2D doorSprite;
    [Export]
    private Condition condition;

            // TODO: In the future, get rid of the export field in "Level" and instead subsribe every door on "_Ready()" to 
            // A static dictionary. When the player goes thru door, the list will reset, the new doors will add to it _Ready()
            // and the old door will check if there's a new door of the same name / number.

    private void OpenDoorOnConditionAchieved() {
        condition.Achieved -= OpenDoorOnConditionAchieved;
        OpenDoor();
    }

    public override void _Ready() {
        opened = false;

        ErrorUtils.AvoidEmptyCollisionLayers(this);
        ErrorUtils.AvoidNullExportedVariables(levelSwitcher, this);
        
        BodyEntered += OnEnterArea;

        // Set up how the door will be opened
        if (condition is null) {
            if (Level.IsCurrentLevelCompleted()) OpenDoor(); 
            else Level.CurrentLevel.LevelCompleted += OpenDoor;
        
        } else {
            condition.Init();
            
            if (condition.IsAchieved)
                OpenDoor();
            else
                condition.Achieved += OpenDoorOnConditionAchieved;
        }
    }

    bool opened;
    public void OpenDoor() {
        
        if (opened) 
            return;

        opened = true;
        doorSprite.Play("open");
    }
    
    string tempName;
    private void OnEnterArea(Node2D body) {
        
        if (!opened || body is not Player) 
            return;

        levelSwitcher.SwitchLevel();
        tempName = Name.ToString(); // Create a copy of the name because otherwise i'll access a disposed object in "UpdateNewDoor"
        PlayerManager.QueueSpawn(Name);
    }

    public Vector2 PlayerSpawnPosition => GlobalPosition + doorOpeningDirection * 100;

    // TODO: Replace with method in animationplayer instead.
	public override void _Process(double delta) {
        if (doorSprite.Frame + 1 == doorSprite.SpriteFrames.GetFrameCount("open")) doorSprite.Pause();
	}
}
