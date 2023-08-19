using Game.Damage;
using Godot;
using System;
using System.Linq;
using Game.Players.Mechanics;
using System.Collections.Generic;

namespace Game.Actors;

public struct Stats {
    public Stats() {}

    public float 
    maxHealthMultipler = 1,
    speedMultiplier = 1,
    healthRegenMultiplier = 1,
    damageTakenMultiplier = 1,

    damageDealthMultiplier = 1,
    reloadSpeedMultiplier = 1;

}


public struct ActorStatsManager {
    public ActorStatsManager(Stats d) {
        defaultStats = d;
    }
    // thees never change
    Stats defaultStats;

    private List<Stats> statChanges = new();
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