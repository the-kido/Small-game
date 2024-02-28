using Godot;
using Game.Data;

namespace Game.UI;

public partial class CoinsLabel : Label {
    [Export]
    AnimationPlayer animationPlayer;

	public override void _Ready() {
        Text = CustomText(RunData.Coins.Count);
        RunData.Coins.ValueChanged += UpdateCoinValue;
    }
    
	public override void _ExitTree() => RunData.Coins.ValueChanged -= UpdateCoinValue;
 
    private static string CustomText(int value) => $" x{value}";

    readonly Color coin = new(0.96f, 0.98f, 0.67f);
    private void UpdateCoinValue(int oldValue, int newValue) {
        animationPlayer.GetAnimation("gained").TrackSetKeyValue(0, 0, newValue > oldValue ? coin : Colors.White);

        animationPlayer.Play("gained");
        
        Text = CustomText(newValue);
        wiggly = 2;
    }

    double wiggly = 0;
    public override void _Process(double delta) {
        
    }
}