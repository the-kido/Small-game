using Game.Data;
using Game.LevelContent;
using Game.Players;
using Godot;

public abstract partial class ShopItem : Sprite2D {

    [Export] 
    Interactable interactable; 

    // in the future don't export but instead get the price based on rarity of item.
    [Export]
    protected int coinAmount;

    public override void _Ready() {
        interactable.Interacted += OnInteracted;
        RunData.AllData[RunDataEnum.Coins].ValueChanged += UpdateInteractableInteractability;
    }

    private void UpdateInteractableInteractability(int newValue) {
        interactable.ProcessMode = newValue >= coinAmount ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
    }

    private void OnInteracted(Player player) {
        RunData.AllData[RunDataEnum.Coins].Add(-coinAmount);
        OnPurchased(player);
    }

    public abstract void OnPurchased(Player player);

    protected void Destroy(Player player) {
        interactable.Destroy(player);
        
        RunData.AllData[RunDataEnum.Coins].ValueChanged -= UpdateInteractableInteractability;
        
        QueueFree();
    }
}