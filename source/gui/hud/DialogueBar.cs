using System;
using Godot;


// This should just make it easier to customize the dialogue
public record ConversationInfo (bool PausePlayerInput = true, bool ShowPortraitImage = true);

public partial class DialogueBar : Control {
    [Export]
    public RichTextLabel Label {get; private set;} 
    [Export]
    public TextureRect PortraitRect;
    [Export]
    private AnimationPlayer animationPlayer;
    public ConversationController ConversationController {get; private set;}
    
    bool showing = false;
    public void Show(bool @bool) {
        // Don't re-play the animation
        if (showing == @bool) return;
        else showing = @bool;

        GD.Print(@bool);
        if (@bool) {
            animationPlayer.Play("Open");
            Visible = true;
        }
        else
            animationPlayer.Play("Close");
    }
    
    public override void _Process(double delta) => ConversationController.Update(delta);
    
    public override void _Ready() {
        ConversationController = new(this);
        
        // When the closing animation finishes playing, "THEN" set the dialogue bar to invisible.
        animationPlayer.AnimationFinished += (name) => {
            if (name == "Close") Visible = false;
        };
    }
}

internal class DialoguePlayer : ConversationPlayer {
    private readonly ConversationController conversationController;
    private readonly DialogueBar bar;
    
    public DialoguePlayer(ConversationController conversationController, DialogueBar bar) {
        this.conversationController = conversationController;
        this.bar = bar;
    }

    double lineProgress;

    // Helpful expression-bodied members used throughout the class
    private bool IsPhraseFinished => bar.Label.VisibleCharacters >= bar.Label.Text.Length;
    char CurrentCharacter => bar.Label.Text[bar.Label.VisibleCharacters];

    public void UpdatePortraitImage(double delta) {
        if (nextLine.portrait.CurrentSprite is null) return;

        nextLine.portrait.PlayAnimation(delta);
        bar.PortraitRect.Texture = nextLine.portrait.CurrentSprite;
    }

    public void Update(double delta) {

        // Update portrait even if the text has stopped typing
        UpdatePortraitImage(delta);

        if (bar.Label.VisibleCharacters == bar.Label.GetParsedText().Length) return;
        
        // Skip spaces.
        if (CurrentCharacter == ' ') lineProgress += 1;
        lineProgress += delta * nextLine.charactersPerSecond;
        
        bar.Label.VisibleCharacters = (int) lineProgress;
    }

    DialogueLine nextLine;
    public void Start(DialogueLine nextLine) {
        this.nextLine = nextLine;
        
        // If asked, do not show the bar
        bar.Show(nextLine.showBar);

        bar.Label.Text = bar.Label.Tr(nextLine.text);
        
        lineProgress = 0;
        bar.Label.VisibleCharacters = 0;
    }

    // Called when click
    public void Skip() {
        if (!IsPhraseFinished) {
            bar.Label.VisibleCharacters = bar.Label.GetParsedText().Length;
        } else {
            conversationController.ContinueConversation();
        }
    }
}

internal class CharacterActionPlayer : ConversationPlayer {
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

    private readonly ConversationController conversationController;
    private readonly DialogueBar bar;

    public CharacterActionPlayer(ConversationController conversationController, DialogueBar bar) {
        this.conversationController = conversationController;
        this.bar = bar;
    }

    CharacterAction action;
    public void Start(CharacterAction action) {
        this.action = action;
        bar.Show(false);

        timerToStopLoopingAnimation = new(action.timeToStopLoopingAnimation);
        timerToStopLoopingAnimation.TimeOver += () => GD.Print("fin");
        timerToStopLoopingAnimation.TimeOver += conversationController.ContinueConversation;
    }
}

internal interface ConversationPlayer {
    void Update(double delta);
}

public class ConversationController {
    // Publicly referable fields / events
    public Action Clicked;
    public event Action<ConversationInfo> Started;
    public event Action Ended;
 
    // Fields
    ConversationItem[] currentDialogue = Array.Empty<ConversationItem>();
    int itemAt = -1;
    
    ConversationItem currentItem => currentDialogue[itemAt];
    private readonly DialogueBar bar;
    
    bool IsConvsersationFinished => currentDialogue.Length == itemAt; 

    public void Update(double delta) {
        if (currentDialogue.Length is 0) return;

        if (currentItem is DialogueLine)
            dialoguePlayer.Update(delta);
        else if (currentItem is CharacterAction)
            characterActionPlayer.Update(delta);
    }


    // rename to "continueDIalouge"
    public void OnClicked() {
        // Make sure that the player cannot click on the dialogue bar as it's going down.
        // if (currentDialogue.Length == 0) return;
       
        if (currentItem is DialogueLine) {
            dialoguePlayer.Skip();
            return;
        }
        if (currentItem is CharacterAction) {
            // do nothing... we will force the player to watch it to completion
            return;
        }
    }
    
    public void Start(ConversationItem[] dialogue, ConversationInfo info) {
        if (dialogue.Length == 0)
            throw new IndexOutOfRangeException("There must be 1 or more ConversationItem's");
        itemAt = -1;
        currentDialogue = dialogue;
        bar.Show(true);

        bar.PortraitRect.Visible = info.ShowPortraitImage;
        Started?.Invoke(info);
        
        ContinueConversation();       
    }

    public void ContinueConversation() {
        // Initialize to the next line, otherwise as soon as the bar opens it will have old stuff on it
        itemAt += 1;

        if (IsConvsersationFinished) {
            Close();
            return;
        }

        GD.Print("continiiing a t", itemAt);
        if (currentItem is DialogueLine line)
            dialoguePlayer.Start(line);
        
        if (currentItem is CharacterAction action)
            characterActionPlayer.Start(action);
    }

    private void Close() {
        Ended?.Invoke();
        
        bar.Label.VisibleCharacters = 0;

        bar.Show(false);
        currentDialogue = Array.Empty<DialogueLine>();
    }

    readonly DialoguePlayer dialoguePlayer;
    readonly CharacterActionPlayer characterActionPlayer;

    public ConversationController(DialogueBar bar) {
        this.bar = bar;
        Clicked += OnClicked;

        dialoguePlayer = new(this, bar);
        characterActionPlayer = new(this, bar);
    }
}
