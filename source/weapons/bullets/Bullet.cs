using Godot;
using System;

public partial class Bullet : Node2D { 

    [Export]
    private int damage;
    [Export(PropertyHint.Range, "-360,360,1,or_greater,or_less")] 
    private int speed;
    private GpuParticles2D particles;

    private void OnArea2DBodyEntered(Node2D body) {
        switch (body) {
            case Enemy e:
                ((Enemy) body).inflictDamage(damage);
                Destory();      
                break;
            case TileMap tm:
                Destory();
                GD.Print("Missed");
                break;
        }
    }
    private void Destory() {
        GetNode<ParticleFactory>("/root/ParticleFactory").SpawnGlobalParticle(particles, GlobalPosition, GlobalRotation + 90);
        //ParticleFactory.spawn_global_particle(particle, global_position, global_rotation+90)
        QueueFree();
    }
    private Vector2 direction;
    public void init(Vector2 spawnPosition, float nuzzleRotation) {
        Rotation = nuzzleRotation;
        direction = new Vector2(Mathf.Cos(nuzzleRotation), Mathf.Sin(nuzzleRotation));
        Position = spawnPosition;
        
    }
    public override void _Ready()
    {
        base._Ready();
        particles = (GpuParticles2D) GetNode("GPUParticles2D");
    }
    public override void _Process(double delta) {
        Position += direction * (float) delta * speed;

    }
 }
