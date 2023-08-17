using Godot;

namespace Game.UI;

public partial class DebugHUD : Control {
    [Export]
    public HSlider anySlider;
    [Export]
    public Button anyButton;
    
    public static DebugHUD instance;

    public DebugHUD() {
        instance = this;
    }
}
