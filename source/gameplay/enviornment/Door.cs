using Godot;
using KidoUtils;

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

    private void OpenDoorOnLevelComplete() =>
        Level.CurrentLevel.LevelCompleted += OpenDoor;
    
    private void OpenDoorOnConditionAchieved() {
        condition.Achieved -= OpenDoorOnConditionAchieved;
        OpenDoor();
    }

    public override void _Ready() {
        opened = false;
        condition.Init();

        ErrorUtils.AvoidEmptyCollisionLayers(this);
        
        BodyEntered += OnEnterArea;

        // Set up how the door will be opened
        if (condition is null)
            Level.LevelStarted += OpenDoorOnLevelComplete;
        else
            if (condition.IsAchieved)
                OpenDoor();
            else
                condition.Achieved += OpenDoorOnConditionAchieved;
    }

    bool opened;
    public void OpenDoor() {
        if (opened) return;

        opened = true;
        doorSprite.Play("default");
    }
    
    string tempName;
    private void OnEnterArea(Node2D body) {
        if (!opened) return;

        if (body is Player) {
            levelSwitcher.SwitchLevel();
            tempName = Name.ToString(); // Create a copy of the name because otherwise i'll access a disposed object in "UpdateNewDoor"
            SceneSwitcher.SceneSwitched += UpdateNewDoor;
        }
    }

    private void SetPlayerAtDoor() {
        Vector2 newPos = GlobalPosition + doorOpeningDirection * 100;
        Player.Players[0].GlobalPosition = newPos;
    }

    private void UpdateNewDoor() {
        SceneSwitcher.SceneSwitched -= UpdateNewDoor;
        Level.CurrentLevel.GetLinkedDoor(tempName).SetPlayerAtDoor();
    }        

    // TODO: Replace with method in animationplayer instead.
	public override void _Process(double delta) {
        if (doorSprite.Frame + 1 == doorSprite.SpriteFrames.GetFrameCount("default")) doorSprite.Pause();
	}
}
