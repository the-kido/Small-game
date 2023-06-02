using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class Effects {

    private static PackedScene Load(string path) => ResourceLoader.Load<PackedScene>(path);

    public static PackedScene 
    Fire = Load("res://assets/effects/fire.tscn"),
    Wet = Load("res://assets/effects/wet.tscn"),
    Gas = Load("res://assets/effects/gas.tscn");

    public static GpuParticles2D AddParticle(Node2D node, PackedScene particle) {
        GpuParticles2D instance = (GpuParticles2D) particle.Instantiate();
        node.AddChild(instance);

        return instance;
    }

    public async static void RemoveParticle(GpuParticles2D particleToRemove) {
        particleToRemove.Emitting = false;

        GD.Print(particleToRemove.GetParent().IsQueuedForDeletion());
        // 5 seconds is p safe time to let all the particles go away before deleted the instance.
        await Task.Delay(5000);
        
        try {
            particleToRemove.QueueFree();
        }
        catch {
            
        }
    }
}
