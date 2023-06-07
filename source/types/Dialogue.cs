using Godot;

public struct DialogueLine {
    public static DialogueLine Empty = new("", Portraits.None);
    public string text;
    public float charactersPerSecond = 10;
    public Portrait portrait; 
    public DialogueLine(string text, Portrait portrait) {
        this.text = text;
        this.portrait = portrait;
    }
}

public struct Portrait {
    public bool loop = false;
    public float fps = 10;

    double progress = 0;

    public Texture2D CurrentSprite => sprites.GetFrameTexture(animationName, currentFrame);
    int currentFrame => (int) Mathf.Floor( (float) progress);
    bool isAnimated => sprites.GetFrameCount(animationName) > 1;
    bool isFinished => currentFrame == (sprites.GetFrameCount(animationName) - 1);


    SpriteFrames sprites;
    string animationName;
    public Portrait(SpriteFrames sprites, string animationName) {
        this.sprites = sprites;
        this.animationName = animationName;
    }

    public void PlayAnimation(double delta) {

        if (!isAnimated) return;
        
        if (isFinished) {
            if (loop) progress = 0;
            else return;
        }

        progress += delta * fps;
    }
}