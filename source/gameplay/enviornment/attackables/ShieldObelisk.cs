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

    KidoUtils.Timer timer = new(5) {loop = true};
    public override void _Ready() {
        base._Ready();
        timer.TimeOver += AddEffectToEveryone;
        Damageable.OnDeath += Uneffect;
    }
    private void Uneffect(DamageInstance damageInstance) {
        timer = new();
        if (Level.CurrentEvent is EnemyWaveEvent enemyWaveEvent) {
            foreach (Enemy enemy in enemyWaveEvent.wave.EnemyChildren)
                enemy.Effect.ClearAllEffects();
        }
    }

    // uhhh
    private void AddEffectToEveryone() {
        if (Level.CurrentEvent is EnemyWaveEvent enemyWaveEvent) {
            foreach (Enemy enemy in enemyWaveEvent.wave.EnemyChildren)
                enemy.Effect.Add(new ShieldedStatus());
        }
    }
    public override void _Process(double delta) {
        timer.Update(delta);
    }
}