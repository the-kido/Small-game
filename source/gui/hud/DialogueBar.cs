using System;
using Godot;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class DialogueBar : Control {
    [Export]
    public RichTextLabel Label {get; private set;} 
    
    [Export]
    public AnimationPlayer animationPlayer;
    
    public void Enable() {
        animationPlayer.Play("Open");
        Visible = true;
    }
    
    public void Disable() {
        animationPlayer.Play("Close");
    }

    public override void _Process(double delta) => dialoguePlayer.Update(delta);
    public override void _Ready() {
        dialoguePlayer.Init(this);
        
        // When the closing animation plays, "THEN" set the dialogue bar to invisible.
        animationPlayer.AnimationFinished += (name) => {
            if (name == "Close") Visible = false;
        };
    }
    public DialoguePlayer dialoguePlayer = new();
}

// ?
public class DialoguePlayer {
    
    public Action OnClicked;
    public void Init(DialogueBar bar) {
        GD.Print("INiT!");
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
    public void Update(double delta) {
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
            GD.Print(lineAt, currentDialogue.Length);
            bar.Label.Text = currentDialogue[lineAt].text;
            progress = 0;
        }

    }
    DialogueLine[] currentDialogue = new DialogueLine[0];

    private void ResetBar(DialogueLine nextText) {
        bar.Label.Text = nextText.text;
        progress = 0;
        lineAt = 0;
        bar.Label.VisibleCharacters = 0;
    }

    public void Start(DialogueLine[] dialogue) {
        currentDialogue = dialogue;
        bar.Enable();
        ResetBar(dialogue[0]);
    }

    private void Close() {
        ResetBar("");
        bar.Disable();
        currentDialogue = new DialogueLine[0];
    }
}

public struct DialogueLine {
    public string text;
    public Portrait portrait; 
    public DialogueLine(string text, Portrait portrait) {
        this.text = text;
        this.portrait = portrait;
    }
}

// How can I make easily referenced portraits that I can use? 
// I only need a handful for a handful of characters. Then, they should be statically referable by
// any class that wishes to use the portrait, whether animated or not.
public struct Portraits {
    
    static SpriteFrames bossSprites => ResourceLoader.Load<SpriteFrames>("");
    static Dictionary<string, Portrait> boss = new() {
        {"Happy", new(bossSprites.Animations) },
    }
}

public struct Portrait {
    public Texture2D CurrentSprite => sprites.GetFrameTexture(animationName, currentFrame);
    //Some animations may loop and are not played once.
    public bool loop = false;
    SpriteFrames sprites;

    int currentFrame => (int) Mathf.Floor( (float) progress);

    bool isAnimated => sprites.GetFrameCount(animationName) > 1;

    string animationName;
    public Portrait(SpriteFrames sprites, string animationName) {
        this.sprites = sprites;
        this.animationName = animationName;
    }

    double progress = 0;
    bool isFinished => currentFrame == (sprites.GetFrameCount(animationName) - 1);
    public void PlayAnimation(double delta) {
        if (!isAnimated) return;
        
        if (isFinished) {
            if (loop) progress = 0;
            else return;
        }

        progress += delta;
    }
}