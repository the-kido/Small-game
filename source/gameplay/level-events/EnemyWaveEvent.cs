using System;
using Godot;

public partial class EnemyWaveEvent : LevelEvent {

    [Export]
    private EnemyWave wave;

    public override event Action Finished;

    public override void Start() {
        wave.CallDeferred("StartWave");
        wave.WaveFinished += test;
    }

    private void test() {
        
        Finished?.Invoke();
    }
}