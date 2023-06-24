using System.Collections.Generic;
using Godot;
using KidoUtils;
using System;

using System.Threading.Tasks;


namespace LootTables;

public record Loot(PackedScene drop, int amount = 1, float chance = 1, bool splashOut = true) {

    public void Init(Node2D node) {
        float random = new Random().NextSingle();
        
        if (chance < random) return;

        for (int i = 0; i < amount; i++) {
            Pickupable pickupable = CreatePickupableInstance(node);
            if (splashOut) pickupable.SplashOut();
        }
    }
    
    private Pickupable CreatePickupableInstance(Node2D node) {
        Pickupable pickupable = drop.Instantiate<Pickupable>();
        pickupable.Position = node.GlobalPosition;
        KidoUtils.Utils.GetPreloadedScene<PickupablesManager>(node, PreloadedScene.PickupablesManager).AddChild(pickupable);

        return pickupable;
    }
}

public class LootTable {

    public static readonly List<Loot> NONE = new List<Loot>();

    public static readonly List<Loot> GENERIC_ENEMY_DROPS = new List<Loot>() {
        new Loot(Coin.PackedScene, 3, 0.5f),
    };

    
}