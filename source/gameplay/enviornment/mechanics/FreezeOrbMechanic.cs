using Game.LevelContent.Criteria;
using Game.Damage;
using Game.Actors;
using Game.ActorStatuses;
using Game.LevelContent;

namespace Game.Mechanics;

public static class FreezeOrbMechanic {
    static DamageInstance GetFreezeDamage(Actor actor) => new(actor) {
        statusEffect = new FreezeEffect(),
        damage = 1, // Just a bit of insult to injury
    };

    public static void Freeze() {
        if (Level.CurrentEvent is EnemyWaveEvent enemyWaveEvent)
            enemyWaveEvent.wave.EnemyChildren.ForEach(child => child.DamageableComponent.Damage(GetFreezeDamage(child)));
    }
}