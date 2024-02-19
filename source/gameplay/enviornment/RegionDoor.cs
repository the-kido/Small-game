// This is special to the door found at spawn
using KidoUtils;
using Godot;
using Game.Players;
using Game.Autoload;

namespace Game.LevelContent;

public partial class RegionDoor : Area2D {

    const string CENTER_REGION_PATH = "res://assets/levels/center_chamber.tscn";

    [Export]
    Regions goToRegion;

    public override void _Ready() {
        BodyEntered += ChangeSceneToFirstLevel;  
    }
    
    private void ChangeSceneToFirstLevel(Node2D body) {
        if (body is Player) {

            if (goToRegion is Regions.CenterRegion) {
                CenterChamber.NotifyForEntery();
                Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPath(CENTER_REGION_PATH);
            }

            string region = RegionManager.Regions[(uint) goToRegion]; 
            Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPackedMap(RegionManager.RegionClasses[region].FirstLevel);
            RegionManager.SetRegion(RegionManager.Regions[(uint) goToRegion]); 
        }
    }
}