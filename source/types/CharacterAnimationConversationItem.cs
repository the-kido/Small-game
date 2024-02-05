using Godot;

namespace Game.Characters;

// Why does this existence. 

/*
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

*/


// Problem:
/*
Animations are not playing b/c update isn't called how do i fix that...
The above class just calls the animation once and that'll work flawlessly 
but that would mean adding more exports to this class
*/