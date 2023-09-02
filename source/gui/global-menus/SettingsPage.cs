
using System;
using System.Runtime.Serialization;
using Godot;

public class Settings {
}

public partial class SettingsPage : ColorRect {
    public void Enable(bool @bool) {
        Visible = @bool;
    }
}