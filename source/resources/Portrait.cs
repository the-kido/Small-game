using Godot;
using System;

public partial class Portrait : Resource  {
    [Export]
    SpriteFrames sprites;
    [Export]
    string animationName;
    
    [ExportGroup("Optional")]
    [Export]
    public bool loopAnimation = false;
    [Export]
    public float fps = 10;
    
    public Texture2D CurrentSprite => sprites?.GetFrameTexture(animationName, CurrentFrame);
    int CurrentFrame => (int) Mathf.Floor( (float) progress);
    bool IsAnimated => sprites.GetFrameCount(animationName) > 1;
    bool IsFinished => CurrentFrame == (sprites.GetFrameCount(animationName) - 1);

    double progress = 0;

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

    public static readonly Portrait None = new(null, "");
}