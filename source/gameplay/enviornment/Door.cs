using Godot;
using KidoUtils;

public partial class Door : Area2D {
    // if there's some sort of condition for the door to open, we of course do not want to open it.
    [Export]
    string nextLevel;

    [Export]
    Vector2I doorOpeningDirection;

    [Export]
    AnimatedSprite2D sprite;

    [Export]
    Condition condition;

            // TODO: In the future, get rid of the export field in "Level" and instead subsribe every door on "_Ready()" to 
            // A static dictionary. When the player goes thru door, the list will reset, the new doors will add to it _Ready()
            // and the old door will check if there's a new door of the same name / number.


    public override void _Ready() {
        ErrorUtils.AvoidEmptyCollisionLayers(this);
        
        BodyEntered += OnEnterArea;

        // Set up how the door will be opened

        if (condition is null) {
            
            Level.Ready += (level) => level.LevelCompleted += OpenDoor;

        } else {
            if (condition.achieved) {
                OpenDoor();
            } else {
                condition.OnConditionAchieved += OpenDoor;
            } 
        }
    }

    bool opened = false;
    public void OpenDoor() {
        // don't open twice. 
        if (opened) return;

        opened = true;
        sprite.Play("default");
    }

    string temp;
    void OnSceneSwitched() {
        temp = Name.ToString();

        SceneSwitcher.SceneSwitched -= OnSceneSwitched;
        Level.Ready += OnLevelReady;
    }

    void OnLevelReady(Level newLevel) {    
        Level.Ready -= OnLevelReady;

        Door newDoor = newLevel.GetLinkedDoor(temp);
        Vector2 newPos = newDoor.GlobalPosition;
        newPos += newDoor.doorOpeningDirection * 100;
        
        Player.players[0].GlobalPosition = newPos;
    }

    private void OnEnterArea(Node2D body) {
        if (!opened) return;
        if (body is Player player) {
            SceneSwitcher sceneSwitcher = KidoUtils.Utils.GetPreloadedScene<SceneSwitcher>(body, PreloadedScene.SceneSwitcher); 
            sceneSwitcher.ChangeSceneWithPath(nextLevel);

            // Ready += OnSceneSwitched;
            SceneSwitcher.SceneSwitched += OnSceneSwitched;
        }
    }

	public override void _Process(double delta) {
        if (sprite.Frame + 1 == sprite.SpriteFrames.GetFrameCount("default")) sprite.Pause();
	}
}
