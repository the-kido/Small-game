using Godot;

public struct DialogueLine {

    public static DialogueLine Empty = new("", Portraits.None);
    public string text;
    public Portrait portrait; 
    public DialogueLine(string text, Portrait portrait) {
        this.text = text;
        this.portrait = portrait;
    }
}

public struct Portrait {
    public Texture2D CurrentSprite => sprites.GetFrameTexture(animationName, currentFrame);
    //Some animations may loop and are not played once.
    public bool loop = false;
    SpriteFrames sprites;
    string animationName;

    int currentFrame => (int) Mathf.Floor( (float) progress);

    bool isAnimated => sprites.GetFrameCount(animationName) > 1;

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

        progress += delta*10;
    }
}