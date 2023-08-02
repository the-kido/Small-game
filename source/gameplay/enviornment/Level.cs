using Godot;
using System;
using System.Threading.Tasks;

public partial class Level : Node {

	public new static event Action<Level> Ready;

    [ExportCategory("Doors")]
    [Export]
    public Godot.Collections.Array<NodePath> doors = new();

    [ExportCategory("Waves")]
    [Export]
    private Godot.Collections.Array<NodePath> waves = new();

    public Door GetLinkedDoor(string name) {
        foreach (NodePath doorPath in doors) {
            Door door = GetNode<Door>(doorPath);

            if (door.Name == name) return door;
        }
        return null;
    }

    public override void _Ready() {
        ChangeLevel();

        isLevelCompleted = LoadLevelCompleted();
        // okay this is really BAD but bear with me here
        if (isLevelCompleted) {
            LevelCompleted?.Invoke();
            return;
        }
        SaveData();

        NextWave();
    }

    private int waveAt = 0;

    public static bool Temp() {
        return CurrentLevel.waves.Count < CurrentLevel.waveAt;
    }
    public static EnemyWave CurrentWave => CurrentLevel.GetNode<EnemyWave>(CurrentLevel.waves[CurrentLevel.waveAt]);

    private async void NextWave() {
        waveAt += 1;

        await Task.Delay(500);

        if (waveAt > waves.Count) {
            LevelCompleted?.Invoke();
            isLevelCompleted = true;
            return;
        }

        CurrentWave.StartWave();
        CurrentWave.WaveFinished += NextWave;
    }

    private void ChangeLevel() {
        CurrentLevel = this;
        waveAt = -1;
        Ready?.Invoke(this);
    }

    public Action LevelCompleted;
    public static Level CurrentLevel {get; private set;} = new();

    bool isLevelCompleted = false;

    private bool LoadLevelCompleted() {
        using FileAccess saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
        string data = saveGame.GetLine();

        if (data == "true") return true;
        else return false;
    }
    private void SaveData() {
        using FileAccess saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
        var jsonString = Json.Stringify(isLevelCompleted);
        saveGame.StoreLine(jsonString);
    }

}

public partial class Level : Node {

    readonly FreezeOrbMechanic freezeOrbMechanic = new();
    public override void _Process(double delta) {
        freezeOrbMechanic.Process(delta);
    }
}

public class FreezeOrbMechanic {

    const int FreezeTime = 5;
    float freezeTimer = FreezeTime;

    static bool isFrozen = false;
    public FreezeOrbMechanic() {
        DungeonRunData.FreezeWave += () => {
            Freeze(true);
            freezeTimer = 0;
        }; 
    }

    public void Process(double delta) {
        if (freezeTimer >= FreezeTime) {
            if (isFrozen)
                Freeze(false);
            return;
        }
        
        freezeTimer += (float) delta;
    }

    private static void Freeze(bool freeze) {
        isFrozen = freeze;

        if (Level.Temp()) return;
        
        Node.ProcessModeEnum processMode = freeze ? Node.ProcessModeEnum.Disabled : Node.ProcessModeEnum.Always;
        // Replace water effect with ice effect obv
        Level.CurrentWave.EnemyChildren.ForEach(child => child.DamageableComponent.Damage(new(child) {statusEffect = new WetStatus()}  ));
        // TODO: Replace this with making the shooting speed multiplier 0 and making its speed multiplier 0 too.
        // that way things like efffects will still apply to it.
        Level.CurrentWave.ProcessMode = processMode;
    }
}