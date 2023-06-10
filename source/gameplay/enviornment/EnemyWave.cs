using Godot;
using System;
public partial class EnemyWave : Node {
    // this will have children of type ENEMY. It defaults to 
    // no process but when the wave starts there "is" a process 


    public event Action WaveFinished;


    public void StartWave() {
        // play cool animations, too.
        ProcessMode = ProcessModeEnum.Always;

        foreach (CharacterBody2D child in GetChildren()) {
            child.Visible = true;
        }
    }

    int totalEnemies; 
    private void ReduceEnemyTotal(DamageInstance _) {
        totalEnemies -= 1;

        if (totalEnemies is 0) {
            WaveFinished?.Invoke();
        }
    }

    public override void _Ready() {
        totalEnemies = GetChildCount();
        
        foreach (Enemy child in GetChildren()) {
            // enforce that the process type is inherited.  
            child.ProcessMode = ProcessModeEnum.Inherit;
            child.DamageableComponent.OnDeath += ReduceEnemyTotal;
            // hide the children
            child.Visible = false;
        }
        
        ProcessMode = ProcessModeEnum.Disabled;
    }
}