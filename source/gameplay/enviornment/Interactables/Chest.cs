using Godot;
using System;
using LootTables;
using System.Diagnostics.Contracts;

public partial class Chest : Interactable {


    [Export]
    string chestLootTable;

    Weapon containedWeapon;

    public override void _Ready() {
        base._Ready();
        float chance = 0;
        float random = new Random().NextSingle();
        foreach (ChestItemDrop item in ChestLootTable.ALL_TABLES[chestLootTable]) {

            chance += item.Chance;
            
            if (chance >= random)
                containedWeapon = item.ChestItem.Instantiate<Weapon>();
        }

    }

    protected override void OnInteracted(Player player) {
        // figure out how i'm gonna randomize this.
        player.GUI.chestMenu.OnWeaponReplaced += (oldWeapon) => {
            AddChild(containedWeapon);
            // would this even work what the
            containedWeapon = oldWeapon;
        };
        
        player.GUI.OpenChestMenu(containedWeapon.packedScene.Instantiate<Weapon>());
        
    }
}