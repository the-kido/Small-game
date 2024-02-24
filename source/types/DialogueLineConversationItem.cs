using Godot;
using Game.Characters;
using System;

namespace Game.UI;

[GlobalClass]
public sealed partial class DialogueLineConversationItem : ConversationItem {
    [Export]
    public string text;
    [Export]
    public Portrait portrait;
    [Export]
    public float charactersPerSecond = 10;
}

public class DialoguePlayer {
    private readonly ConversationController conversationController;
    private readonly DialogueBar bar;
    public DialoguePlayer(ConversationController conversationController, DialogueBar bar) {
        this.conversationController = conversationController;
        this.bar = bar;
    }

    double lineProgress;
    private bool IsPhraseFinished => bar.Label.VisibleCharacters >= bar.Label.GetParsedText().Length; 
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

    DialogueLineConversationItem nextLine;
    public void Start(DialogueLineConversationItem nextLine) {
        this.nextLine = nextLine;

        if (nextLine.charactersPerSecond is 0) throw new ArgumentOutOfRangeException(nextLine.charactersPerSecond.ToString(), "The charactersPerSecond exported field for DialougeLineConversationItem must be greater than 0!");
        if (nextLine.portrait is null) throw new NullReferenceException("There was no portrait set for this dialogue line!");
        
        bar.Show(true);
        bar.Label.Text = bar.Label.Tr(nextLine.text);
        
        lineProgress = 0;
        bar.Label.VisibleCharacters = 0;
    }

    // Called when click
    public void Skip() {
        if (!IsPhraseFinished) bar.Label.VisibleCharacters = bar.Label.GetParsedText().Length;
        else conversationController.ContinueConversation();
    }
}