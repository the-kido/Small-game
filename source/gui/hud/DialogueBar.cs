using System;
using Godot;
using System.Threading.Tasks;
using System.Collections.Generic;


// This should just make it easier to customize the dialogue
public record DialogueInfo (bool PausePlayerInput = true, bool ShowPortraitImage = true);

public partial class DialogueBar : Control {
    [Export]
    public RichTextLabel Label {get; private set;} 
    [Export]
    public TextureRect PortraitRect;
    [Export]
    private AnimationPlayer animationPlayer;
    public DialoguePlayer DialoguePlayer {get; private set;}
    
    public void Show(bool @bool) {
        if (@bool)
            animationPlayer.Play("Open");
        else
            animationPlayer.Play("Close");
    }
    
    public override void _Process(double delta) => DialoguePlayer.Update(delta);
    
    public override void _Ready() {
        DialoguePlayer = new(this);
        
        // When the closing animation finishes playing, "THEN" set the dialogue bar to invisible.
        animationPlayer.AnimationFinished += (name) => {
            if (name == "Close") Visible = false;
        };
    }
}

public class DialoguePlayer {
    // Publicly referable fields / events
    public Action OnClicked;
    public event Action<DialogueInfo> DialogueStarted;
    public event Action DialogueEnded;

    // Fields
    DialogueLine[] currentDialogue = Array.Empty<DialogueLine>();
    private readonly DialogueBar bar;
    int lineAt = 0;
    double lineProgress;

    // Helpful expression-bodied members used throughout the class
    private bool IsPhraseFinished => bar.Label.VisibleCharacters >= bar.Label.Text.Length;
    private bool IsDialogueFinished => currentDialogue.Length - 1 == lineAt && IsPhraseFinished;
    char CurrentCharacter => bar.Label.Text[bar.Label.VisibleCharacters];

    public void UpdatePortraitImage(double delta) {
        if (currentDialogue[lineAt].portrait.CurrentSprite is null) return;

        currentDialogue[lineAt].portrait.PlayAnimation(delta);
        bar.PortraitRect.Texture = currentDialogue[lineAt].portrait.CurrentSprite;
    }

    public void Update(double delta) {
        // If there's no dialogue, then there's no point in updating.
        if (currentDialogue.Length == 0) return;

        // Update portrait even if the text has stopped typing
        UpdatePortraitImage(delta);

        if (bar.Label.VisibleCharacters == bar.Label.GetParsedText().Length) return;
        
        // Skip spaces.
        if (CurrentCharacter == ' ') lineProgress += 1;
        lineProgress += delta * currentDialogue[lineAt].charactersPerSecond;
        
        bar.Label.VisibleCharacters = (int) lineProgress;
    }

    public void ContinueDialogue() {
        // Make sure that the player cannot click on the dialogue bar as it's going down.
        if (currentDialogue.Length == 0) return;
        
        if (IsDialogueFinished) {
            Close();
            return;
        }

        if (!IsPhraseFinished) {
            bar.Label.VisibleCharacters = bar.Label.GetParsedText().Length;
        } else {
            lineAt += 1;
            UpdateToNextLine();
        }
    }
    private void UpdateToNextLine() {
        // If asked, do not show the bar
        bar.Show(currentDialogue[lineAt].showBar);
        
        bar.Label.Text = bar.Label.Tr(currentDialogue[lineAt].text);
        
        lineProgress = 0;
        bar.Label.VisibleCharacters = 0;
    }


    public void Start(DialogueLine[] dialogue, DialogueInfo info) {
        bar.PortraitRect.Visible = info.ShowPortraitImage;
        DialogueStarted?.Invoke(info);
        
        currentDialogue = dialogue;
        bar.Show(true);
        
        // Initialize to the next line, otherwise as soon as the bar opens it will have old stuff on it
        UpdateToNextLine();
    }

    private void Close() {
        DialogueEnded?.Invoke();

        lineProgress = 0;
        lineAt = 0;
        bar.Label.VisibleCharacters = 0;

        bar.Show(false);
        currentDialogue = Array.Empty<DialogueLine>();
    }

    public DialoguePlayer(DialogueBar bar) {
        this.bar = bar;
        OnClicked += ContinueDialogue;
    }
}
