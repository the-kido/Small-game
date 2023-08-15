using Godot;
using System;
using LootTables;
using System.Security.AccessControl;

public partial class Chest : Sprite2D {

    [Export]
    string chestLootTable;

    [Export]
    Interactable interactable;

    [Export]
    Sprite2D itemShowcase;

    IChestItem containedWeapon;

    public override void _Ready() {
        interactable.Interacted += OnInteracted;

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
                Disable(containedWeaponInstance.Icon);
            }
            return;
        }

        if (containedWeapon.Type is ChestItemType.SHIELD) {

        }
    }

    Timer timer = Timer.NONE;
    private void Disable(Texture2D sprite) {
        interactable.QueueFree();
        Modulate = new(0.8f, 0.8f, 0.9f);
        
        itemShowcase.Texture = sprite;
        
        timer = new(5);
        timer.TimeOver += FullyDisable;
    }
    // fantastic
    private void FullyDisable() {
        itemShowcase.QueueFree();
        timer = Timer.NONE;
    }

    public override void _Process(double delta) => timer.Update(delta);

    private void OnInteracted(Player player) {
        
        switch (containedWeapon.Type) {
            case ChestItemType.WEAPON:
                for (int i = 0; i < player.WeaponManager.Weapons.Length; i++) {
                    Weapon weapon = player.WeaponManager.Weapons[i];
                    if (weapon is null) {
                        player.WeaponManager.AddWeapon((Weapon) containedWeapon, i);
                        Disable(containedWeapon.Icon);
                        return;
                    }
                }
                break;
            case ChestItemType.SHIELD:
                if (player.ShieldManager.HeldShield is null) {
                    player.ShieldManager.ChangeShield((Shield) containedWeapon);
                    Disable(containedWeapon.Icon);
                    return;
                }
                break;
        }

        player.GUI.chestMenu.OnSelectionMade += (oldWeaponIndex) => SwitchItems(oldWeaponIndex, player);

        player.GUI.OpenChestMenu(containedWeapon);
    }
}