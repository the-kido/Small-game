using Godot;
using Game.ActorStatuses;
using Game.LevelContent;
using Game.Actors;
using Game.LevelContent.Criteria;
using Game.Damage;
using Game.Players;

namespace Game.SealedContent;

public sealed partial class ShieldObelisk : BreakableObject, IPlayerAttackable {       
    public Vector2 GetPosition() => GlobalPosition;
    public bool IsInteractable() => Damageable.IsAlive;
    public Node2D GetNode() => this;
    
    public override void _Ready() {
        base._Ready();
        Damageable.OnDeath += Uneffect;
        
        Level.CriterionStarted += AddEffectToEveryone;
        
        // In case the criterion already started, check again
        if (Level.CurrentCriterion is not null)
            AddEffectToEveryone(Level.CurrentCriterion);
        
    }

    public override void _ExitTree() {
        Level.CriterionStarted -= AddEffectToEveryone;
        Damageable.OnDeath -= Uneffect;
    }

    private static void Uneffect(DamageInstance damageInstance) {
        if (Level.CurrentCriterion is EnemyWaveEvent enemyWaveEvent) {
            foreach (Enemy enemy in enemyWaveEvent.wave.EnemyChildren) {
                enemy.Effect.ClearAllEffects();
            }
        }
    }

    private static void AddEffectToEveryone(LevelCriteria levelCriteria) {
        if (levelCriteria is EnemyWaveEvent enemyWaveEvent) {
            foreach (Enemy enemy in enemyWaveEvent.wave.EnemyChildren) 
                enemy.Effect.Add(new ShieldedStatus());
        }
    }
}