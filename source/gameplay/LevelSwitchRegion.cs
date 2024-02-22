using Godot;
using KidoUtils;
using Game.Players;
using Game.Data;
using Game.Autoload;

namespace Game.LevelContent;

public partial class LevelSwitchRegion : Area2D {
    
    [Export(PropertyHint.File, "*.tscn,")]
    public string nextLevel;
    [Export]
    private Vector2I doorOpeningDirection;
    [Export]
    Door doorToOpen; 

    public void SwitchLevel() {
        SceneSwitcher sceneSwitcher = Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher); 
        sceneSwitcher.ChangeSceneWithPath(nextLevel);
    }

    [ExportCategory("Deprecated")]
    [Export]
    private Condition condition;

    private void OpenDoorOnConditionAchieved() {
        condition.Achieved -= OpenDoorOnConditionAchieved;
        PlayDoorAnimation();
    }

    public override void _Ready() {
        opened = false;

        ErrorUtils.AvoidEmptyCollisionLayers(this);
        
        BodyEntered += OnEnterArea;

        // Set up how the door will be opened
        if (condition is null) {
            if (Level.IsCurrentLevelCompleted()) ShowDoorAsOpen(); 
            else Level.CurrentLevel.LevelCompleted += PlayDoorAnimation;
        
        } else {
            condition.Init();
            
            if (condition.IsAchieved) ShowDoorAsOpen();
            else condition.Achieved += OpenDoorOnConditionAchieved;
        }
    }

    bool opened;

    // This assumes the door is already open.
    public void ShowDoorAsOpen() {
        doorToOpen.InstantlyOpen();
    }

    public void PlayDoorAnimation() {
        doorToOpen.Open();
    }
    
    string tempName;
    private void OnEnterArea(Node2D body) {
        
        if (!opened || body is not Player) return;

        SwitchLevel();
        tempName = Name.ToString(); // Create a copy of the name because otherwise i'll access a disposed object in "UpdateNewDoor"
        PlayerManager.QueueSpawn(Name);
    }

    public Vector2 PlayerSpawnPosition => GlobalPosition + doorOpeningDirection * 100;
}
