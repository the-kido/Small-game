// This is special to the door found at spawn
using KidoUtils;
using Godot;
using Game.Players;
using Game.Autoload;

namespace Game.LevelContent;

public partial class SpawnDoor : Area2D {
    [Export(PropertyHint.File, "*.tscn,")]
    public string firstLevel;
    public override void _Ready() {
        BodyEntered += ChangeSceneToFirstLevel;  
    }
    private void ChangeSceneToFirstLevel(Node2D body) {
        if (body is Player)
            Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPath(firstLevel);
    }
}