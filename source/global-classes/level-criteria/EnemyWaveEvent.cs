using System;
using Godot;

namespace Game.LevelContent.Criteria;

[GlobalClass]
public partial class EnemyWaveEvent : LevelCriteria {

    [Export]
    public EnemyWave wave;

    public override event Action Finished;

    public override void Start() {
        wave.CallDeferred("StartWave");
        wave.WaveFinished += () => Finished?.Invoke();
    }
}