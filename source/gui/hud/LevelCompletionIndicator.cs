using Godot;
using Game.LevelContent;

namespace Game.UI;

public partial class LevelCompletionIndicator : Control{
    [Export]
    AnimationPlayer animationPlayer;

    public override void _Ready() {
        if (Level.LevelCompletions[Level.CurrentLevel.SaveName]) Enable(); 
        else Level.CurrentLevel.LevelCompleted += Enable;
    }

    private void Enable(){
        Level.CurrentLevel.LevelCompleted -= Enable;
        animationPlayer.Play("Open");
    }        
}