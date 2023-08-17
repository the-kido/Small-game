using Godot;
using Game.Characters;

namespace Game.UI;

public partial class DialogueLine : ConversationItem {
    [Export]
    public string text;
    [Export]
    public Portrait portrait;

    [ExportGroup("Optional")]
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
        
        bar.Show(true);

        bar.Label.Text = bar.Label.Tr(nextLine.text);
        
        lineProgress = 0;
        bar.Label.VisibleCharacters = 0;
    }

    // Called when click
    public void Skip() {
        if (!IsPhraseFinished)
            bar.Label.VisibleCharacters = bar.Label.GetParsedText().Length;
        else
            conversationController.ContinueConversation();
    }
}