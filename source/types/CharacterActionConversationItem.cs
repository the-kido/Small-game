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

[GlobalClass]
public sealed partial class CharacterActionConversationItem : ConversationItem {
    [Export]
    // this is temporary. this class should have an "animation player" instance within it i think?
    public Character character;
    [Export]
    public StringName animationName;
    
    [Export]
    public bool playAmbiently; // Plays the animation in the background
    [Export]
    public float loopTime;

    [ExportGroup("Optional")]
    [Export]
    public Vector2 moveToPosition = new(); // Moves the character to this positon while playing the given animation


    private CharacterActionPlayer a;
    public void ProcessPlayer(CharacterActionPlayer a) {
        this.a = a;
    }

    public override void _Process(double delta) => a?.Update(delta);
}

public class CharacterActionPlayer {

    private readonly ConversationController conversationController;
    private readonly DialogueBar bar;

    private CharacterActionConversationItem action;
    public CharacterActionPlayer(ConversationController conversationController, DialogueBar bar) {
        this.conversationController = conversationController;
        this.bar = bar;
    }

    StringName AnimationName => action.animationName;
    AnimationPlayer AnimationPlayer => action.character.AnimationPlayer;
    KidoUtils.Timer loopTimer = KidoUtils.Timer.NONE;


    public void Start(CharacterActionConversationItem action) {
        this.action = action;
        action.ProcessPlayer(this); // i am testing right now!11

        if (!action.playAmbiently) bar.Show(false);

        if (action.loopTime is not 0) {
            AnimationPlayer.GetAnimation(AnimationName).LoopMode = Godot.Animation.LoopModeEnum.Linear;
            loopTimer = new(action.loopTime);
            
            loopTimer.TimeOver += () => {
                AnimationPlayer.Stop();
                loopTimer = KidoUtils.Timer.NONE;
            };
            
        } else {
            AnimationPlayer.GetAnimation(AnimationName).LoopMode = Godot.Animation.LoopModeEnum.None;
        } 

        AnimationPlayer.Play(AnimationName);
        
        if (action.playAmbiently) {
            conversationController.ContinueConversation();
        } else {
            if (action.loopTime is not 0) loopTimer.TimeOver += conversationController.ContinueConversation;
            else AnimationPlayer.AnimationFinished += ContinueOnAnimationFinished;
        }
    }

    public void Update(double delta) {

        if (action.moveToPosition != Vector2.Zero) {
            action.character.MoveTo(delta, action.moveToPosition);
        }

        if (action.loopTime is not 0) {
            loopTimer.Update(delta);
        }
        
        if (!loopTimer.Equals(KidoUtils.Timer.NONE)) {
            loopTimer.Update(delta);
        }
    }

    private void TimerFinished() {
        
    }

    private void ContinueOnAnimationFinished(StringName stringName) {
        AnimationPlayer.AnimationFinished -= ContinueOnAnimationFinished;
        conversationController.ContinueConversation();
    }
}