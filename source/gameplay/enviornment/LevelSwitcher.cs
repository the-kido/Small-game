using Godot;
using KidoUtils;

public partial class LevelSwitcher : Node {
    [Export(PropertyHint.File, "*.tscn,")]
    string nextLevel;
    public void SwitchLevel() {
        SceneSwitcher sceneSwitcher = Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher); 
        sceneSwitcher.ChangeSceneWithPath(nextLevel);
    }
}