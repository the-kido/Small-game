using Godot;

public partial class BulletVisual : Node2D {
    
    [Export]
    Sprite2D sprite;
    
    [Export]
    GpuParticles2D deathParticle;

    [Export]
    GpuParticles2D persistentParticle;

}