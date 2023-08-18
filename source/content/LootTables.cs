using System.Collections.Generic;
using Godot;
using KidoUtils;
using System;
using Game.SealedContent;
using Game.Autoload;
using Game.LevelContent.Pickupables;

namespace LootTables;

public record ItemDrop(PackedScene Drop, int Amount = 1, float Chance = 1, bool SplashOut = true) {

    // I really don't wanna reinstance / reallocate space for these a whole bunch of times which is why it's outside the init method :/
    // I know, it's very ugly...
    readonly Random random = new();

    float randomFloat;
    
    public void Init(Node2D node) {

        for (int i = 0; i < Amount; i++) {

            randomFloat = random.NextSingle();
            if (Chance < randomFloat) continue;

            Pickupable pickupable = CreatePickupableInstance(node);
            if (SplashOut) pickupable.SplashOut();
        }
    }
    
    private Pickupable CreatePickupableInstance(Node2D node) {
        Pickupable pickupable = Drop.Instantiate<Pickupable>();
        pickupable.Position = node.GlobalPosition;
        Utils.GetPreloadedScene<PickupablesManager>(node, PreloadedScene.PickupablesManager).AddChild(pickupable);

        return pickupable;
    }
}

public static class EnemyLootTable {

    public static readonly List<ItemDrop> NONE = new();

    public static readonly List<ItemDrop> GENERIC_ENEMY_DROPS = new() {
        new(Coin.PackedScene, 3, 0.5f),
    };
}

public record ChestItemDrop(PackedScene ChestItem, float Chance);


public static class ChestLootTables {
 
    public static readonly Dictionary<ChestTables, List<ChestItemDrop>> All = new() {
        {ChestTables.ChargedGun, new() {
            new(ChargedGun.PackedSceneResource, 1f)
            }
        },
        {ChestTables.BadShield, new() {
            new(BadShield.PackedSceneResource, 1f),
            }
        }
    };
}   

public enum ChestTables : uint {
    ChargedGun = 0,
    BadShield = 1,
}