using Godot;
using System.Collections.Generic;

public static class DialogueLines {
    // public static readonly Dictionary<string, DialogueLine[]> Lines = new() {
    //     // Agro boss
    //     {
    //         "Kill Agro Boss", new DialogueLine[] {
    //             new("AGRO ENEMY DEATH-1", Portraits.boss["Happy"]),
    //             new("AGRO ENEMY DEATH-2", Portraits.boss["Sad"]),
    //         }
    //     },
    //     {
    //         "Interact with Boulder", new DialogueLine[] {
    //             new("BOULDER-1", Portraits.None),
    //             new("BOULDER-2", Portraits.None),
    //         }
    //     },
    // };
}

public partial class DialogueLine : Node {
    public static readonly DialogueLine Hidden = new("", Portrait.None) {showBar = false};

    [Export]
    public string text;
    [Export]
    public Portrait portrait;

    [ExportGroup("Optional")]
    [Export]
    public float charactersPerSecond = 10;
    [Export]
    public bool showBar = true;
    public DialogueLine(string text, Portrait portrait) {
        this.text = text;
        this.portrait = portrait;
    }
}
