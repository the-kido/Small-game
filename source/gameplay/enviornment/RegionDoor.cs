using KidoUtils;
using Godot;
using Game.Players;
using Game.Autoload;

namespace Game.LevelContent;

public partial class RegionDoor : Area2D {
    
    [Export]
    Regions goToRegion;
    
    [Export]
    Door door;
    
    public override void _Ready() {
        BodyEntered += SwitchScene; 
        int index = (int) goToRegion - 1;
        
        if (index < 0 && goToRegion is Regions.Dungeon) door.Open();
        else if (RegionManager.RegionsWon[index]) door.Open();
    }
    
    //there are 2 ways to get to the center:
    // death / winning.
    // if you die, it should not reset the LevelCompleted to false
    private void SwitchScene(Node2D body) {
        if (body is not Player) return;

        
        if (goToRegion is Regions.Center) {
            CenterChamber.NotifyForEnteryAfterWinning();
            Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPath(RegionManager.CENTER_REGION_PATH);
            RegionManager.RegionWon();
        } else {
            // Enter the new dungeon 
            Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher).ChangeSceneWithPackedMap(RegionManager.GetRegionClass(goToRegion).FirstLevel);
        }
        RegionManager.SetRegion(goToRegion);
    }
}