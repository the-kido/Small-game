using Game.Data;

namespace Godot.Data;

// Required for the statue. There will be a sliding door which takes this condiition and when you get enough coins it lets you pass.
public partial class ValueCondition : Condition {

    [ExportGroup("Value-bound condition")]
    [Export]
    private RunDataEnum runData;
    [Export]
    private int valueToAchieve;

    public ValueCondition() : base() {
        RunData.GetRunDataFromEnum(runData).ValueChanged += CheckForAchievement;
    }
    
    private void CheckForAchievement(int newValue) {
        if (newValue >= valueToAchieve) 
            Achieve();
    }
}