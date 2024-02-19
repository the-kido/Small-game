using KidoUtils;
using Godot;
using Game.Players;
using Game.Autoload;
using Game.Data;
using System.Linq;

namespace Game.LevelContent;

public partial class RegionDoor : Area2D {
    
    

    [Export]
    Regions goToRegion;
    
    public override void _Ready() {
        BodyEntered += ChangeSceneToFirstLevel;  
    }
    
    //there are 2 ways to get to the center:
    // death / winning.
    // if you die, it should not reset the LevelCompleted to false
    private void ChangeSceneToFirstLevel(Node2D body) {
        if (body is not Player) return;

        if (goToRegion is Regions.CenterRegion) {
            CenterChamber.NotifyForEnteryAfterWinning();
            RegionManager.RegionWon();
            Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPath(RegionManager.CENTER_REGION_PATH);
        } else {
            string region = RegionManager.Regions[(uint)goToRegion];
            // Enter the new dungeon 
            Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPackedMap(RegionManager.RegionClasses[region].FirstLevel);
            RegionManager.SetRegion(RegionManager.Regions[(uint)goToRegion]);
        }
    }
}