using Godot;

public partial class CoinsLabel : Label {

	public override void _Ready() {
        Text = CustomText(DungeonRunData.Coins);
        DungeonRunData.CoinValueChanged += UpdateCoinValue;
    }
    
	public override void _ExitTree() => DungeonRunData.CoinValueChanged -= UpdateCoinValue;
 
    private string CustomText(int value) => $"Coins: {value}";

    private void UpdateCoinValue(int newValue) {
        Text = CustomText(newValue);
    }
}