using Game.Damage;
using Godot;
using System;
using System.Linq;
using Game.Players.Mechanics;
using System.Collections.Generic;

namespace Game.Actors;

// These apply for enemies, players, etc, but will be utilized however fit.
public struct ActorStats {
    public ActorStats() {}

    public float 
    maxHealthMultipler = 1,
    speedMultiplier = 1,
    healthRegenMultiplier = 1,
    damageTakenMultiplier = 1,

    damageDealthMultiplier = 1,
    reloadSpeedMultiplier = 1;
}


public struct ActorStatsManager {
    public ActorStatsManager(ActorStats d) {
        defaultStats = d;
    }
    // thees never change
    ActorStats defaultStats;

    private List<ActorStats> statChanges = new();

    // used to call "update stats or whatevr it's called"
    public event Action<ActorStats> statsChanged;
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