using Godot;

public partial class CharacterAnimation : ConversationItem {
    [Export]
    public Character character;
    [Export]
    public StringName animationName;
}

public class CharacterAnimationPlayer {
    public void Start(CharacterAnimation characterAnimation) {
        characterAnimation.character.AnimationPlayer.Play(characterAnimation.animationName);
    }
}
