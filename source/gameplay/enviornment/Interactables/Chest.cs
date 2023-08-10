using Godot;
using System;
using LootTables;
using System.Diagnostics.Contracts;

public partial class Chest : Interactable {

    [Export]
    string chestLootTable;

    IChestItem containedWeapon;

    public override void _Ready() {
        base._Ready();
        float chance = 0;
        float random = new Random().NextSingle();

        foreach (ChestItemDrop item in ChestLootTable.ALL_TABLES[chestLootTable]) {

            chance += item.Chance;
            
            if (chance >= random)
                containedWeapon = item.ChestItem.Instantiate<IChestItem>();
        }
    }

    private void SwitchItems(int oldWeaponIndex, Player player) {

        if (containedWeapon.Type is ChestItemType.WEAPON) {
            Weapon containedWeaponInstance = ((Weapon) containedWeapon).PackedScene.Instantiate<Weapon>();
            Weapon oldWeapon = player.WeaponManager.GetWeapon(oldWeaponIndex);

            player.WeaponManager.AddWeapon(containedWeaponInstance, oldWeaponIndex);
            containedWeapon = oldWeapon?.PackedScene.Instantiate<IChestItem>();

            if (containedWeapon is null) {
                QueueFree();
            }
            return;
        }

        if (containedWeapon.Type is ChestItemType.SHIELD) {

        }
    }

    protected override void OnInteracted(Player player) {
        
        switch (containedWeapon.Type) {
            case ChestItemType.WEAPON:
                for (int i = 0; i < player.WeaponManager.Weapons.Length; i++) {
                    Weapon weapon = player.WeaponManager.Weapons[i];
                    if (weapon is null) {
                        player.WeaponManager.AddWeapon((Weapon) containedWeapon, i);
                        QueueFree();
                        return;
                    }
                }
                break;
            case ChestItemType.SHIELD:
                if (player.ShieldManager.HeldShield is null) {
                    player.ShieldManager.ChangeShield((Shield) containedWeapon);
                    QueueFree();
                    return;
                }
                break;
        }

        // figure out how i'm gonna randomize this.
        player.GUI.chestMenu.OnSelectionMade += (oldWeaponIndex) => SwitchItems(oldWeaponIndex, player);

        player.GUI.OpenChestMenu(containedWeapon);
    }
}