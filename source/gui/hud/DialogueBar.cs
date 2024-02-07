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

// Controls the passage of dialogue during a "
public class ConversationController {
    // Publicly referable fields / events
    public Action Clicked;
    public event Action<ConversationInfo> Started;
    public event Action Ended;
 
    // Fields
    ConversationItem[] currentDialogue = Array.Empty<ConversationItem>();
    int itemAt = -1;
    
    ConversationItem CurrentItem => currentDialogue[itemAt];
    private readonly DialogueBar bar;
    
    bool IsConvsersationFinished => currentDialogue.Length == itemAt; 

    public void Update(double delta) {
        if (currentDialogue.Length is 0) return;
        
        switch (CurrentItem) {
            case DialogueLineConversationItem:
                dialoguePlayer.Update(delta);
                break;
                
        }
    }

    // rename to "continueDIalouge"
    public void ContinueDialogue() {
        // Make sure that the player cannot click on the dialogue bar as it's going down.
        if (currentDialogue.Length is 0) return;
       
        if (CurrentItem is DialogueLineConversationItem) {
            dialoguePlayer.Skip();
            return;
        }
    }
    
    public void Start(ConversationItem[] dialogue, ConversationInfo info) {
        if (dialogue.Length is 0) throw new IndexOutOfRangeException("There must be 1 or more ConversationItem's");
        
        itemAt = -1;
        currentDialogue = dialogue;

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

        if (CurrentItem is DialogueLineConversationItem line) 
            dialoguePlayer.Start(line);
        
        if (CurrentItem is CharacterActionConversationItem action) {
            characterActionPlayer.Start(action);
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
    // readonly CharacterAnimationPlayer characterAnimationPlayer;

    public ConversationController(DialogueBar bar) {
        this.bar = bar;
        Clicked += ContinueDialogue;

        dialoguePlayer = new(this, bar);
        characterActionPlayer = new(this, bar);
        // characterAnimationPlayer = new();
    }
}
