using System;
using Godot;
using Game.Characters;

namespace Game.UI;

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

        if (@bool) {
            animationPlayer.Play("Open");
            Visible = true;
        }
        else
            animationPlayer.Play("Close");
    }
    
    public override void _Process(double delta) => 
        ConversationController.Update(delta);
    
    public override void _Ready() {
        ConversationController = new(this);
        
        // When the closing animation finishes playing, "THEN" set the dialogue bar to invisible.
        animationPlayer.AnimationFinished += (name) => {
            if (name == "Close") Visible = false;
        };
    }
}

// TODO: Add a way to call an animation that doesn't close the 
// bar but still loops or plays an animation and QUICKLY
// Such that it is basically "part of" the next line being spokened

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

        if (currentItem is DialogueLineConversationItem)
            dialoguePlayer.Update(delta);
        else if (currentItem is CharacterActionConversationItem)
            characterActionPlayer.Update(delta);
    }

    // rename to "continueDIalouge"
    public void OnClicked() {
        // Make sure that the player cannot click on the dialogue bar as it's going down.
        if (currentDialogue.Length == 0) return;
       
        if (currentItem is DialogueLineConversationItem) {
            dialoguePlayer.Skip();
            return;
        }
        if (currentItem is CharacterActionConversationItem) {
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

        if (currentItem is DialogueLineConversationItem line) 
            dialoguePlayer.Start(line);
        
        if (currentItem is CharacterActionConversationItem action)
            characterActionPlayer.Start(action);

        // Play the animation, then quickly continue to the next thingy
        if (currentItem is CharacterAnimationConversationItem characterAnimation) {
            characterAnimationPlayer.Start(characterAnimation);
            ContinueConversation();
        }
    }

    private void Close() {
        Ended?.Invoke();
        
        bar.Label.VisibleCharacters = 0;

        bar.Show(false);
        currentDialogue = Array.Empty<DialogueLineConversationItem>();
    }

    readonly DialoguePlayer dialoguePlayer;
    readonly CharacterActionPlayer characterActionPlayer;
    readonly CharacterAnimationPlayer characterAnimationPlayer;

    public ConversationController(DialogueBar bar) {
        this.bar = bar;
        Clicked += OnClicked;

        dialoguePlayer = new(this, bar);
        characterActionPlayer = new(this, bar);
        characterAnimationPlayer = new();
    }
}
