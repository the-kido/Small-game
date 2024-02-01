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
        RunData.Coins.ValueChanged += UpdateInteractableInteractability;
    }

    private void UpdateInteractableInteractability(int newValue) {
        interactable.ProcessMode = newValue >= Price ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
    }

    public override void _ExitTree() {
        RunData.Coins.ValueChanged -= UpdateInteractableInteractability;
    }

    private void OnInteracted(Player player) {
        RunData.Coins.Add(-Price);
        OnPurchased(player);
        
        DisconnectEvents(player);
        QueueFree();

        // Save any purchases made
        if (Level.IsCurrentLevelCompleted())
            GameDataService.Save(); 
    }

    public abstract void OnPurchased(Player player);

    public void DisconnectEvents(Player player) {
        interactable.Destroy(player);
        RunData.Coins.ValueChanged -= UpdateInteractableInteractability;
    }
}