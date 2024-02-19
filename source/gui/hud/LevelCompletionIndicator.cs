using Godot;
using Game.LevelContent;

namespace Game.UI;

public partial class LevelCompletionIndicator : Control {
    [Export]
    AnimationPlayer animationPlayer;

    public void Init() {
        if (Level.CurrentCriterion is null || CenterChamber.WithinCenterChamber) return;
        
        if (Level.IsCurrentLevelCompleted()) Enable(); 
        else Level.CurrentLevel.LevelCompleted += Enable;
    }

    private void Enable() {
        Level.CurrentLevel.LevelCompleted -= Enable;
        animationPlayer.Play("Open");
    }        
}