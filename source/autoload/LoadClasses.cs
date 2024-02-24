using Game.LevelContent;
using Godot;

public partial class LoadClasses : Node {
    
    public override void _EnterTree() {
		_ = Level.CurrentLevel; // inits levels
		_ = Level.lastLevelPlayedSaver; // inits gamedataservice
		_ = RegionManager.RegionsWon; // inits regionmanager
    }
}