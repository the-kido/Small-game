using Godot;
using Game.Data;

namespace Game.UI;

public partial class CoinsLabel : Label {

	public override void _Ready() {
        Text = CustomText(RunData.Coins.Count);
        RunData.Coins.ValueChanged += UpdateCoinValue;
    }
    
	public override void _ExitTree() => RunData.Coins.ValueChanged -= UpdateCoinValue;
 
    private static string WigglyCustomText(int value) => $"[wave][] x{value}";
    private static string CustomText(int value) => $" x{value}";

    private void UpdateCoinValue(int old, int newValue) {
        Text = CustomText(newValue);
        wiggly = 2;
    }

    double wiggly = 0;
    public override void _Process(double delta) {

    }
}