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

    public ShieldObelisk() {
        Level.CriterionStarted += AddEffectToEveryone;
    }
    
    public override void _Ready() {
        base._Ready();
        Damageable.OnDeath += Uneffect;
    }

    private void Uneffect(DamageInstance damageInstance) {
        if (Level.CurrentEvent is EnemyWaveEvent enemyWaveEvent) {
            foreach (Enemy enemy in enemyWaveEvent.wave.EnemyChildren) {
                GD.Print("CLEANING");
                enemy.Effect.ClearAllEffects();
            }
        }
    }

    private void AddEffectToEveryone(LevelCriteria levelCriteria) {
        if (levelCriteria is EnemyWaveEvent enemyWaveEvent) {
            foreach (Enemy enemy in enemyWaveEvent.wave.EnemyChildren) 
                enemy.Effect.Add(new ShieldedStatus());
        }
    }
}