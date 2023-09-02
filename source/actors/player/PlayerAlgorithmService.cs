using Game.Actors;
using Game.Damage;

namespace Game.Players;

// Idk if this will be useful but if I add more algorithms it might be needed for organization
public class PlayerAlgorithmService {
    readonly AggressionMetric agressionMetric;
    
    public PlayerAlgorithmService (Player player) {
        agressionMetric = new(player);
    }
}

public class AggressionMetric {
    public double Agression {get; set;}
    readonly Player player;
    
    KidoUtils.Timer timer;
    public AggressionMetric(Player player) {
        this.player = player;
        Enemy.EnemyKilled += OnPlayerKilledEnemy;
        timer = new(5);
    }

    // Depending on how much health the enemy has, the score should go up by a bit.

    int score;
    private void OnPlayerKilledEnemy(Enemy enemyKilled, DamageInstance killingDamageInstance) {
        score += enemyKilled.DamageableComponent.BaseMaxHealth;
    }
}