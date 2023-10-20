using Game.Players;
using Game.Players.Mechanics;
using Godot;
using LootTables;

public sealed partial class WeaponShopItem : ShopItem {
    [Export]
	ChestTables chestLootTable;

    IChestItem heldWeapon;


    public override void _Ready() {
        base._Ready();
        heldWeapon = ChestLootTables.Roll(chestLootTable);
        Texture = heldWeapon.Icon;
    }

    public override void OnPurchased(Player player) {
        if (heldWeapon is Weapon weapon) 
            player.WeaponManager.AddAndSwitchWeapon(weapon, player.WeaponManager.SelectedSlot);
        if (heldWeapon is Shield shield) 
            player.ShieldManager.ChangeShield(shield);
        
        DisconnectEvents(player);
        QueueFree();
    }
}

