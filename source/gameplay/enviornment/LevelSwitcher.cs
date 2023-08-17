using Godot;
using KidoUtils;
using Game.Autoload;

namespace Game.LevelContent;

public partial class LevelSwitcher : Node {
    [Export(PropertyHint.File, "*.tscn,")]
    string nextLevel;
    public void SwitchLevel() {
        SceneSwitcher sceneSwitcher = Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher); 
        sceneSwitcher.ChangeSceneWithPath(nextLevel);
    }
}