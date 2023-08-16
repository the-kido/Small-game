using Godot;
using KidoUtils;

public partial class Door : Area2D {
    // if there's some sort of condition for the door to open, we of course do not want to open it.

    [Export]
    LevelSwitcher levelSwitcher;

    [Export]
    Vector2I doorOpeningDirection;

    [Export]
    AnimatedSprite2D doorSprite;

    [Export]
    Condition condition;

            // TODO: In the future, get rid of the export field in "Level" and instead subsribe every door on "_Ready()" to 
            // A static dictionary. When the player goes thru door, the list will reset, the new doors will add to it _Ready()
            // and the old door will check if there's a new door of the same name / number.

    public override void _Ready() {
        condition.Init();

        ErrorUtils.AvoidEmptyCollisionLayers(this);
        
        BodyEntered += OnEnterArea;

        // Set up how the door will be opened
        if (condition is null) {
            Level.LevelStarted += () => Level.CurrentLevel.LevelCompleted += OpenDoor;
        } else {
            if (condition.IsAchieved) {
                OpenDoor();
            } else {
                condition.Achieved += OpenDoor;
            } 
        }
    }

    bool opened = false;
    public void OpenDoor() {
        // don't open twice. 
        if (opened) return;

        opened = true;
        doorSprite.Play("default");
    }

    string temp;
    void OnSceneSwitched() {
        temp = Name.ToString();

        SceneSwitcher.SceneSwitched -= Temp;
        Level.LevelStarted += OnLevelReady;
    }

    void OnLevelReady() {    
        Level.LevelStarted -= OnLevelReady;

        Door newDoor = Level.CurrentLevel.GetLinkedDoor(temp);
        Vector2 newPos = newDoor.GlobalPosition;
        newPos += newDoor.doorOpeningDirection * 100;
        
        Player.Players[0].GlobalPosition = newPos;
    }

    private void OnEnterArea(Node2D body) {
        if (!opened) return;

        if (body is Player) {
            levelSwitcher.SwitchLevel();
            
            SceneSwitcher.SceneSwitched += Temp;
        }
    }
    
    private void Temp() {
        CallDeferred("OnSceneSwitched");
    }
    // TODO: Replace with method in animationplayer instead.
	public override void _Process(double delta) {
        if (doorSprite.Frame + 1 == doorSprite.SpriteFrames.GetFrameCount("default")) doorSprite.Pause();
	}
}
