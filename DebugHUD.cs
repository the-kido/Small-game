using Godot;
using System;

public partial class DebugHUD : Control
{
    [Export]
    public HSlider anySlider;
    public static DebugHUD instance;

    public DebugHUD() {
        instance = this;
    }
}
