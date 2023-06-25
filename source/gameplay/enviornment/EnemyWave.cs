using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class EnemyWave : Node {
    // this will have children of type ENEMY. It defaults to 
    // no process but when the wave starts there "is" a process 
    
    static private readonly PackedScene enemySpawnIndicator = ResourceLoader.Load<PackedScene>("res://source/autoload/enemy_spawn_indicator.tscn");

    public event Action WaveFinished;


    readonly List<Node2D> indicators = new();
    public async void StartWave() {

        // play cool animations, too.

        foreach (Enemy child in EnemyChildren) {
            Node2D instance = enemySpawnIndicator.Instantiate<Node2D>();
            Level.CurrentLevel.AddChild(instance);
            indicators.Add(instance);
            instance.GlobalPosition = child.GlobalPosition;
        }

        await Task.Delay(1000);

        ProcessMode = ProcessModeEnum.Always;

        foreach (Enemy child in EnemyChildren) {
            SetChildVisibility(child, true);
        }

        indicators.ForEach(indicator => Level.CurrentLevel.RemoveChild(indicator));
    }

    int totalEnemies; 
    private void ReduceEnemyTotal(DamageInstance _) {
        totalEnemies -= 1;

        if (totalEnemies is 0) {
            WaveFinished?.Invoke();
        }
    }

    private static void SetChildVisibility(Enemy child, bool isEnabled) {
        child.Visible = isEnabled;
        child.CollisionShape.Disabled = !isEnabled;
    }

    public readonly List<Enemy> EnemyChildren = new();

    public override void _Ready() {
        totalEnemies = GetChildCount();
        
        foreach (Enemy child in GetChildren()) {
            EnemyChildren.Add(child);
            // enforce that the process type is inherited. 
            child.DamageableComponent.OnDeath += ReduceEnemyTotal;
            child.ProcessMode = ProcessModeEnum.Inherit;

            SetChildVisibility(child, false);
        }
        
        ProcessMode = ProcessModeEnum.Disabled;
    }
}