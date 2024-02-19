using System;
using Game.Autoload;
using Game.LevelContent.Criteria;
using Godot;
using KidoUtils;

// might be useful?
[GlobalClass]
public partial class ChangeSceneEvent : LevelCriteria {
    public override event Action Finished;
    [Export]
    PackedScene sceneToSwitchTo;

    public override void Start() {
        Finished?.Invoke();
        SceneSwitcher sceneSwitcher = Utils.GetPreloadedScene<SceneSwitcher>(this, PreloadedScene.SceneSwitcher); 
        sceneSwitcher.ChangeSceneWithPackedMap(sceneToSwitchTo);
    }
}