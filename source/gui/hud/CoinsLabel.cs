using Godot;
using Game.Data;

namespace Game.UI;

public partial class CoinsLabel : Label {

	public override void _Ready() {
        Text = CustomText(RunData.Coins.Count);
        RunData.Coins.ValueChanged += UpdateCoinValue;
    }
    
	public override void _ExitTree() => RunData.Coins.ValueChanged -= UpdateCoinValue;
 
    private string CustomText(int value) => $"Coins: {value}";

    private void UpdateCoinValue(int newValue) {
        Text = CustomText(newValue);
    }
}