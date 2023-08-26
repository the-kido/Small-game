using Godot;

namespace Game.Characters;

[GlobalClass]
public sealed partial class CharacterAnimationConversationItem : ConversationItem {
    [Export]
    public Character character;
    [Export]
    public StringName animationName;
}

public class CharacterAnimationPlayer {
    public void Start(CharacterAnimationConversationItem characterAnimation) {
        characterAnimation.character.AnimationPlayer.Play(characterAnimation.animationName);
    }
}
