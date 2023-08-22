using Godot;
using System;
using System.Linq;
using Game.Players.Mechanics;
using System.Collections.Generic;

namespace Game.Actors;

// These apply for enemies, players, etc, but will be utilized however fit.
public class ModifiedStat {
    public float multiplier = 1;
    public float adder = 0;

    public ModifiedStat() {}
    public ModifiedStat(float multiplier, float adder) {
        if (multiplier < 0) throw new ArgumentOutOfRangeException("The multiplier argument in the ModifiedStat constructor cannot be less than 0");
        this.multiplier = multiplier;
        this.adder = adder;
    }

    public void Add(ModifiedStat statPair) {
        multiplier *= statPair.multiplier;
        adder += statPair.adder;
    }
    public void Remove(ModifiedStat statPair) {
        multiplier /= statPair.multiplier;
        adder -= statPair.adder;
    }

    public float GetEffectiveValue(float baseValue) => baseValue * multiplier + baseValue * adder;
}
public struct ActorStats {

    public ActorStats() {}

    public ModifiedStat 
    maxHealth = new(1, 0),
    speed = new(1, 0),
    regenSpeed = new(1, 0),
    damageTaken = new(1, 0),
    damageDealt = new(1, 0),
    reloadSpeed = new(1, 0);
}

public struct ActorStatsManager {
    public ActorStatsManager(ActorStats d, Action<ActorStats> invokedMethod) {
        defaultStats = d;
        StatsChanged = invokedMethod;
    }

    public void AddStats(ActorStats stats) {
        statChanges.Add(stats);

        newStats.maxHealth.Add(stats.maxHealth);
        newStats.speed.Add(stats.speed);
        newStats.regenSpeed.Add(stats.regenSpeed);
        newStats.damageTaken.Add(stats.damageTaken);
        newStats.damageDealt.Add(stats.damageDealt);
        newStats.reloadSpeed.Add(stats.reloadSpeed);
        
        StatsChanged?.Invoke(newStats);
    }

    public void RemoveStats(ActorStats stats) {
        statChanges.Remove(stats);

        newStats.maxHealth.Remove(stats.maxHealth);
        newStats.speed.Remove(stats.speed);
        newStats.regenSpeed.Remove(stats.regenSpeed);
        newStats.damageTaken.Remove(stats.damageTaken);
        newStats.damageDealt.Remove(stats.damageDealt);
        newStats.reloadSpeed.Remove(stats.reloadSpeed);

        StatsChanged?.Invoke(newStats);
    }

    // thees never change
    ActorStats defaultStats = new();
    ActorStats newStats = new();

    private List<ActorStats> statChanges = new();

    // used to call "update stats or whatevr it's called"
    public event Action<ActorStats> StatsChanged;
}

public partial class PlayerClass : Resource {
    [Export]
    AnimatedSprite2D playerSprites;
    [Export]
    Weapon defaultWeapon;
    [Export]
    Shield defaultShield;

    //stats
    [Export]
    int maxHealth;
    // [Export]
    // Stats stats;

    static readonly string[] RequiredAnimations = {"north", "east", "south", "west"};
    // Temporary i need this to screen this resource for errors
    public void Init() {
        foreach (string name in playerSprites.SpriteFrames.Animations) {
            if (!RequiredAnimations.Contains(name))
                throw new NullReferenceException($"The animations {name} is not found within the resource {ResourcePath}");
        }
    }
}