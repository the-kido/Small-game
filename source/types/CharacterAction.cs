using Godot;
using Game.UI;

// An action can mean:
// - Moving to a point
// - Playing an animation once
// - Playing an animation several times

namespace Game.Characters;

// This should just make it easier to customize the dialogue
public record ConversationInfo (bool PausePlayerInput = true, bool ShowPortraitImage = true);
public abstract partial class ConversationItem : Node {}

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

public class CharacterActionPlayer {

    private readonly ConversationController conversationController;
    private readonly DialogueBar bar;

    private CharacterAction action;
    public CharacterActionPlayer(ConversationController conversationController, DialogueBar bar) {
        this.conversationController = conversationController;
        this.bar = bar;
    }

    public void Start(CharacterAction action) {
        this.action = action;
        bar.Show(false);

        timerToStopLoopingAnimation = new(action.timeToStopLoopingAnimation);
        timerToStopLoopingAnimation.TimeOver += conversationController.ContinueConversation;
    }

    public void Update(double delta) {

        if (action.moveToPosition != Vector2.Zero) {
            MoveTo(delta);
            return;
        }

        if (action.timeToStopLoopingAnimation is not 0) {
            timerToStopLoopingAnimation.Update(delta);
            return;
        }

        // If the other criteria above pass, the only option left is to play the animation once.
        PlayAnimationOnce();
    }

    // these are the 3 ways that this thing can be doned
    #region #1
    private void MoveTo(double delta) {
        bool finished = action.character.MoveTo(delta, action.moveToPosition);
        if (finished) conversationController.ContinueConversation();
    }
    #endregion #1

    #region #2
    private void PlayAnimationOnce() {
        if (action.character.AnimationPlayer.IsPlaying()) return;
        
        action.character.AnimationPlayer.Play(action.animationName);
        action.character.AnimationPlayer.AnimationFinished += AnimationFinished;
    }
    private void AnimationFinished(StringName stringName) {
        action.character.AnimationPlayer.AnimationFinished -= AnimationFinished;
        conversationController.ContinueConversation();
    }
    #endregion #2

    #region #3
    Timer timerToStopLoopingAnimation;
    #endregion #3
}