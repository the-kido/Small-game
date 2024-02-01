using Game.Data;
using Game.Players;

public sealed partial class RespawnTokenShopItem : ShopItem {
    public override void _Ready() {
        base._Ready();
        interactable.Description = "Allows you to retry a stage instead of being thrown back to the start";
    }
    
    public override void OnPurchased(Player player) {   
        RunData.RespawnTokens.Add(1);
    }
}
