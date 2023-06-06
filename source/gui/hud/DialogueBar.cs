using System;
using Godot;
using System.Threading.Tasks;
using System.Collections.Generic;


// This should just make it easier to customize the dialogue
public struct DialogueInfo {
    public bool pausePlayerInput = true;

	public DialogueInfo() {}
}

public partial class DialogueBar : Control {
    [Export]
    public RichTextLabel Label {get; private set;} 
    [Export]
    public TextureRect PortraitRect;
    [Export]
    public AnimationPlayer animationPlayer;
    
    public void Enable() {
        animationPlayer.Play("Open");
        Visible = true;
    }
    
    public void Disable() {
        animationPlayer.Play("Close");
    }

    public override void _Process(double delta) => player.Update(delta);
    public override void _Ready() {
        player.Init(this);
        
        // When the closing animation finishes playing, "THEN" set the dialogue bar to invisible.
        animationPlayer.AnimationFinished += (name) => {
            if (name == "Close") Visible = false;
        };
    }
    public DialoguePlayer player = new();
}

// ?
public class DialoguePlayer {

    public Action OnClicked;

    public event Action<DialogueInfo> DialogueStarted;
    public event Action DialogueEnded;

    public void Init(DialogueBar bar) {
        this.bar = bar;
        OnClicked += ContinueDialogue;
    }

    //used to continue dialogue
    private bool IsPhraseFinished => bar.Label.VisibleCharacters >= bar.Label.Text.Length;

    //used for... closing gui ?!
    private bool IsDialogueFinished => currentDialogue.Length - 1 == lineAt && IsPhraseFinished;

    private DialogueBar bar;
    int lineAt = 0;
    double progress;

    char currentCharacter => bar.Label.Text[bar.Label.VisibleCharacters];

    public void UpdatePortraitImage(double delta) {
        currentDialogue[lineAt].portrait.PlayAnimation(delta);
        bar.PortraitRect.Texture = currentDialogue[lineAt].portrait.CurrentSprite;
    }

    public void Update(double delta) {
        // If there's no dialogue, then there's no point in updating.
        if (currentDialogue.Length == 0) return;

        UpdatePortraitImage(delta);
        if (bar.Label.VisibleCharacters == bar.Label.GetParsedText().Length) return;

        //bar.Label.VisibleCharacters = (int) MathF.Min(bar.Label.VisibleCharacters, bar.Label.Text.Length);
        
        // Skip spaces.
        if (currentCharacter == ' ') progress += 1;
        progress += delta * 10;
        
        bar.Label.VisibleCharacters = (int) progress;
    }

    public void ContinueDialogue() {
        if (IsDialogueFinished) {
            Close();
            return;
        }

        if (!IsPhraseFinished) {
            bar.Label.VisibleCharacters = bar.Label.GetParsedText().Length;
        } else {
            // Go to the next line.
            lineAt += 1;
            bar.Label.Text = currentDialogue[lineAt].text;
            progress = 0;
        }

    }
    DialogueLine[] currentDialogue = new DialogueLine[0];

    private void ResetBar(DialogueLine next) {
        bar.Label.Text = next.text;
        progress = 0;
        lineAt = 0;
        bar.Label.VisibleCharacters = 0;
        bar.PortraitRect.Texture = next.portrait.CurrentSprite;
    }

    public void Start(DialogueLine[] dialogue, DialogueInfo info) {
        DialogueStarted?.Invoke(info);
        currentDialogue = dialogue;
        bar.Enable();
        ResetBar(dialogue[0]);
    }

    private void Close() {
        DialogueEnded?.Invoke();
        progress = 0;
        lineAt = 0;
        bar.Label.VisibleCharacters = 0;

        bar.Disable();
        currentDialogue = new DialogueLine[0];
    }
}
