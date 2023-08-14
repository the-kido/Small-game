using Godot;

// An action can mean:
// - Moving to a point
// - Playing an animation once
// - Playing an animation several times

public partial class CharacterAction : ConversationItem {
    [Export]
    // this is temporary. this class should have an "animation player" instance within it i think?
    public Character character;
    [Export]
    public StringName animationName;
    
    [ExportGroup("Optional")]
    [Export]
    public Vector2 moveToPosition = new();
    [Export]
    public float timeToStopLoopingAnimation;
}