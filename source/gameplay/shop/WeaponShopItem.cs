using System;
using Godot;
using LootTables;

public sealed partial class WeaponShopItem : ShopItem {
    [Export]
	ChestTables chestLootTable;

    IChestItem heldWeapon;

    public override void _Ready() {
        base._Ready();

        float chance = 0;
		float random = new Random().NextSingle();

		foreach (ChestItemDrop item in ChestLootTables.All[chestLootTable]) {

			chance += item.Chance;
			
			if (chance >= random)
				heldWeapon = item.ChestItem.Instantiate<IChestItem>();
		}

        Texture = heldWeapon.Icon;
    }

    public override void OnPurchased() {
        GD.Print("we do a bit of trollolol");
    }
}