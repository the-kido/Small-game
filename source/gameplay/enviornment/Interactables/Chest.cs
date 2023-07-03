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

    private void A(Weapon oldWeapon, Player player) {
        containedWeapon = oldWeapon;
        player.GUI.chestMenu.OnWeaponReplaced -= (oldWeapon) => A(oldWeapon, player);
    }
    
    protected override void OnInteracted(Player player) {
        // figure out how i'm gonna randomize this.
        player.GUI.chestMenu.OnWeaponReplaced += (oldWeapon) => A(oldWeapon, player);
        
        player.GUI.OpenChestMenu(containedWeapon.PackedScene.Instantiate<Weapon>());
        
    }
}