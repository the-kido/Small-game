using Godot;
using Game.LevelContent;

namespace Game.UI;

public partial class LevelCompletionIndicator : Control {
    [Export]
    AnimationPlayer animationPlayer;

    public void Init() {
        if (Level.CurrentEvent is null) return;

        if (Level.CurrentLevelCompleted()) Enable(); 
        else Level.CurrentLevel.LevelCompleted += Enable;
    }

    private void Enable() {
        Level.CurrentLevel.LevelCompleted -= Enable;
        animationPlayer.Play("Open");
    }        
}