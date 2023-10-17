using Game.Data;
using Game.LevelContent;
using Game.Players;
using Godot;

public abstract partial class ShopItem : Sprite2D {

    [Export] 
    Interactable interactable; 

    // in the future don't export but instead get the price based on rarity of item.
    [Export]
    private int coinAmount;

    public override void _Ready() {
        interactable.Interacted += OnInteracted;

        RunData.AllData[RunDataEnum.Coins].ValueChanged += UpdateInteractableInteractability;
    }

    private void UpdateInteractableInteractability(int newValue) {
        interactable.ProcessMode = newValue >= coinAmount ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
    }

    private void OnInteracted(Player player) {
        RunData.AllData[RunDataEnum.Coins].Add(-coinAmount);
        OnPurchased();
    }

    public abstract void OnPurchased();

}