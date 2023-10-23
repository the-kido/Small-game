using Game.Players;
using Godot;

public sealed partial class PotionShopItem : ShopItem {
    [Export]
    int HealAmount;

    public override void _Ready() {
        base._Ready();
        interactable.Description = $"Heal {HealAmount} health";
    }
    
    public override void OnPurchased(Player player) {
        player.DamageableComponent.Heal(HealAmount);
    }
}
