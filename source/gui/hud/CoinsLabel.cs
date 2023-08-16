using Godot;

public partial class CoinsLabel : Label {

	public override void _Ready() {
        Text = CustomText(DungeonRunData.Coins.Count);
        DungeonRunData.Coins.ValueChanged += UpdateCoinValue;
    }
    
	public override void _ExitTree() => DungeonRunData.Coins.ValueChanged -= UpdateCoinValue;
 
    private string CustomText(int value) => $"Coins: {value}";

    private void UpdateCoinValue(int newValue) {
        Text = CustomText(newValue);
    }
}