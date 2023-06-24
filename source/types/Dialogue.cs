using Godot;
using System.Collections.Generic;




public static class DialogueLines {


    public static readonly Dictionary<string, DialogueLine[]> lines = new()
    {

        // Agro boss
        {"Kill Agro Boss", new DialogueLine[] {
            new("AGRO ENEMY DEATH-1", Portraits.boss["Happy"]),
            new("AGRO ENEMY DEATH-2", Portraits.boss["Sad"]),
        }

        },
    };

}

public struct DialogueLine {
    public static readonly DialogueLine Empty = new("", Portraits.None);
    public string text;
    public float charactersPerSecond = 10;
    public Portrait portrait; 
    public DialogueLine(string text, Portrait portrait) {
        
        this.text = text;
        this.portrait = portrait;
        
    }
}

public struct Portrait {
    public bool loopAnimation = false;
    public float fps = 10;

    double progress = 0;

    readonly public Texture2D CurrentSprite => sprites.GetFrameTexture(animationName, CurrentFrame);

    readonly int CurrentFrame => (int) Mathf.Floor( (float) progress);
    readonly bool IsAnimated => sprites.GetFrameCount(animationName) > 1;
    readonly bool IsFinished => CurrentFrame == (sprites.GetFrameCount(animationName) - 1);


    readonly SpriteFrames sprites;
    readonly string animationName;
    public Portrait(SpriteFrames sprites, string animationName) {
        this.sprites = sprites;
        this.animationName = animationName;
    }

    public void PlayAnimation(double delta) {

        if (!IsAnimated) return;
        
        if (IsFinished) {
            if (loopAnimation) progress = 0;
            else return;
        }

        progress += delta * fps;
    }
}