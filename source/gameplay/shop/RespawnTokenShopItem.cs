using Game.Data;
using Game.Players;

public sealed partial class RespawnTokenShopItem : ShopItem {
    public override void _Ready() {
        base._Ready();
    }
    
    public override void OnPurchased(Player player) {   
        RunData.AllData[RunDataEnum.RespawnTokens].Add(1);
    }
}
