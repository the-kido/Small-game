using Godot;
using System;
using System.Collections.Generic;
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

        NextWave();
    }
    int waveAt = 0;
    private async void NextWave() {

        await Task.Delay(500);

        if (waveAt > waves.Count - 1) {
            LevelCompleted?.Invoke();
            return;
        }

        GetNode<EnemyWave>(waves[waveAt]).StartWave();
        GetNode<EnemyWave>(waves[waveAt]).WaveFinished += NextWave;
        waveAt += 1;

    }

    private void ChangeLevel() {
        currentLevel = this;
        
        Ready?.Invoke(this);
    }

    public Action LevelCompleted; 
    public static Level currentLevel = new();

    //public Dictionary<string, Door> doors2 = new();

}