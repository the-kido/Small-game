using Godot;
using System;
using System.Linq;
using Game.Players.Mechanics;
using System.Collections.Generic;
using Game.ActorStatuses;

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

    public readonly List<ModifiedStat> List => new() {
        maxHealth,
        speed,
        regenSpeed,
        damageTaken,
        damageDealt,
        reloadSpeed
    };
}

public struct ActorStatsManager {

    // used to call "update stats or whatevr it's called"
    public event Action<ActorStats> StatsChanged;

    // Keeps track of all stat changes 
    private readonly List<ActorStats> statChanges = new();

    public ActorStatsManager(Action<ActorStats> invokedMethod) {
        StatsChanged = invokedMethod;
    }

    public void AddStats(ActorStats stats) {
        statChanges.Add(stats);
        UpdateStatValues();
    }

    public void RemoveStats(ActorStats stats) {
        statChanges.Remove(stats);
        UpdateStatValues();
    }

    public void UpdateStatValues() {
        ActorStats newStats = new();

        foreach (var statChange in statChanges) {
            for (int i = 0; i < statChange.List.Count; i++) {
                ModifiedStat stat = statChange.List[i];
                newStats.List[i].Add(stat);
            }
        }
        StatsChanged?.Invoke(newStats);
    }
}

[GlobalClass]
public partial class PlayerClassResource : Resource {
    [Export]
    public SpriteFrames playerSprites {get; private set;}
    [Export]
    public PackedScene defaultWeapon {get; private set;}
    [Export]
    public PackedScene defaultShield {get; private set;}

    // Idk if there's a better solution to this...
    [Export]
    public int maxHealthOverride {get; private set;}

    static readonly string[] RequiredAnimations = {"north", "east", "south", "west"};
    // Temporary i need this to screen this resource for errors

    public void DoSafetyChecks() {
        foreach (string animationName in RequiredAnimations) {
            if (!playerSprites.HasAnimation(animationName))
                throw new NullReferenceException($"The animations {animationName} is not found within the resource {ResourcePath}");
        }
    }
}