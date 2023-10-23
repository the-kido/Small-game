using Game.Data;
using Game.LevelContent;
using Game.Players;
using Godot;

public abstract partial class ShopItem : Sprite2D {

    [Export] 
    protected Interactable interactable; 

    // in the future don't export but instead get the price based on rarity of item.
    [Export]
    public int Price {get; private set;}

    public override void _Ready() {
        interactable.Interacted += OnInteracted;
        RunData.AllData[RunDataEnum.Coins].ValueChanged += UpdateInteractableInteractability;
    }

    private void UpdateInteractableInteractability(int newValue) {
        interactable.ProcessMode = newValue >= Price ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
    }

    private void OnInteracted(Player player) {
        RunData.AllData[RunDataEnum.Coins].Add(-Price);
        OnPurchased(player);
    }

    public abstract void OnPurchased(Player player);

    public void DisconnectEvents(Player player) {
        interactable.Destroy(player);
        RunData.AllData[RunDataEnum.Coins].ValueChanged -= UpdateInteractableInteractability;
    }
}