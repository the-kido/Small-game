using Godot;

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
    

    public Texture2D CurrentSprite => sprites.GetFrameTexture(animationName, CurrentFrame);
    int spriteCount => sprites.GetFrameCount(animationName);
    int CurrentFrame => (int) Mathf.Floor( (float) progress);
    bool IsAnimated => spriteCount > 1;
    bool IsFinished => CurrentFrame == spriteCount;

    double progress = 0;

    public void PlayAnimation(double delta) {

        if (!IsAnimated) return;
        
        progress += delta * fps;
        
        if (IsFinished) {
            if (loopAnimation) progress = 0;
            else progress = spriteCount - 1;
        }
    }
}