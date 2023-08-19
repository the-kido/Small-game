using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Game.Actors;
using Game.Damage;
using KidoUtils;

namespace Game.LevelContent;

[GlobalClass]
public partial class EnemyWave : Node {
    // this will have children of type ENEMY. It defaults to 
    // no process but when the wave starts there "is" a process 
    
    static private readonly PackedScene enemySpawnIndicator = ResourceLoader.Load<PackedScene>("res://source/autoload/enemy_spawn_indicator.tscn");

    public event Action WaveFinished;

    private KidoUtils.Timer showEnemyTimer = KidoUtils.Timer.NONE;
    // NOTE: In the EnemyWaveEvent class, it is called via CallDefered
    // This is because not all of the children get ready before the "wave" node does. 
    public void StartWave() {
        List<Node2D> indicators = new();
        showEnemyTimer = new(1) { loop = false};

        // play cool animations, too.
        foreach (Enemy child in EnemyChildren) {
            Node2D instance = enemySpawnIndicator.Instantiate<Node2D>();
            Level.CurrentLevel.GetParent().AddChild(instance);
            indicators.Add(instance);
            instance.GlobalPosition = child.GlobalPosition;
        }

        showEnemyTimer.TimeOver += () => ShowEnemies(indicators);
    }

    private void ShowEnemies(List<Node2D> indicators) {

        foreach (Enemy child in EnemyChildren) {
            SetChildVisibility(child, true);
        }

        indicators.ForEach(indicator => Level.CurrentLevel.GetParent().RemoveChild(indicator));
    }

    public override void _Process(double delta) => showEnemyTimer.Update(delta);

    int totalEnemies; 
    private void ReduceEnemyTotal(DamageInstance _) {
        totalEnemies -= 1;

        if (totalEnemies is 0)
            WaveFinished?.Invoke();

    }

    private static void SetChildVisibility(Enemy child, bool isEnabled) {
        child.Visible = isEnabled;
        child.CollisionShape.Disabled = !isEnabled;
        child.ProcessMode = isEnabled ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
    }

    public List<Enemy> EnemyChildren => GetChildren().Cast<Enemy>().ToList();

    public override void _Ready() {
        totalEnemies = GetChildCount();
        
        foreach (Enemy child in GetChildren()) {
            // enforce that the process type is inherited. 
            child.DamageableComponent.OnDeath += ReduceEnemyTotal;

            SetChildVisibility(child, false);
        }
    }
}