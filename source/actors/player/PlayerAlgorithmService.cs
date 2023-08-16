using System;

// Idk if this will be useful but if I add more algorithms it might be needed for organization
public class PlayerAlgorithmService {
    readonly AgressionMetric agressionMetric;
    
    public PlayerAlgorithmService (Player player) {
        agressionMetric = new(player);
    }

}

public class AgressionMetric {
    public double Agression {get; set;}
    readonly Player player;
    
    Timer timer;
    public AgressionMetric(Player player) {
        this.player = player;
        DungeonRunData.EnemiesKilled.EnemyKilled += OnPlayerKilledEnemy;
        timer = new(5);
    }

    // Depending on how much health the enemy has, the score should go up by a bit.

    int score;
    private void OnPlayerKilledEnemy(Enemy enemyKilled) {
        score += enemyKilled.DamageableComponent.MaxHealth;
           
    }
}