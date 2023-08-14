using Godot;

public abstract partial class ConversationItem : Node {}

public partial class DialogueLine : ConversationItem {
    // public static readonly DialogueLine Hidden = new("", Portrait.None) {showBar = false};

    [Export]
    public string text;
    [Export]
    public Portrait portrait;

    [ExportGroup("Optional")]
    [Export]
    public float charactersPerSecond = 10;
    [Export]
    public bool showBar = true;

}
