using System;
using Godot;

public partial class DialogueBar : Control {
    public event Action Disable;
    [Export]
    public RichTextLabel Label {get; private set;} 

    public void Enable() {
        foreach (var func in Disable.GetInvocationList()) {
            Disable -= (func as Action);
        }
        Visible = true;
    }
    
    public void Switch() {
        Visible = false;
    }
}

// ?
public static class Dialogue {
    private static DialogueBar bar {
        get {
            return Player.players[0].GUI.HUD.dialogueBar;
        }
    }

    public static void Start(string text) {
        bar.Label.Text = text;
    }
}