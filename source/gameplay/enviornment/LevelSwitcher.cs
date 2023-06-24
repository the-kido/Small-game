using Godot;
using KidoUtils;

public partial class LevelSwitcher : Node {
    [Export]
    string nextLevel;
    public void SwitchLevel() {
        SceneSwitcher sceneSwitcher = KidoUtils.Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher); 
        sceneSwitcher.ChangeSceneWithPath(nextLevel);
    }
}