using Game.Players;
using Game.SealedContent;
using Godot;

namespace Game.UI;

public partial class DebugHUD : Control {
    [Export]
    public HSlider anySlider;

    [Export]
    public Button anyButton;
    
    public static DebugHUD instance;

    public override void _Ready() {
        // anyButton.Pressed += () => Player.Players[0].DamageableComponent.Damage(new(new AgroEnemyMiniBoss()) {damage = 4, overridesImmunityFrames = true});
        anyButton.Pressed += () => Player.Players[0].DamageableComponent.Heal(10);
    }
    
    public void Init() {
        instance = this;
    }
}
