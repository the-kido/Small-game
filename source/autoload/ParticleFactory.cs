using Godot;
using KidoUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class ParticleFactory : Node
{
	// Called when the node enters the scene tree for the first time.
	private static Node factoryNode;
	public override void _Ready() {
		factoryNode = Utils.GetPreloadedScene<ParticleFactory>(this, PreloadedScene.ParticleFactory);
	}

	public static GpuParticles2D AddParticle(Node2D node, PackedScene particle) {
        GpuParticles2D instance = (GpuParticles2D) particle.Instantiate();
        node.AddChild(instance);
        return instance;
    }
	public async static void RemoveParticle(GpuParticles2D particleToRemove) {
        particleToRemove.Emitting = false;
        
        // 5 seconds is p safe time to let all the particles go away before deleted the instance.
        await Task.Delay(5000);

		// Stop updating this particle's position. 
		if (updatedParticles.ContainsKey(particleToRemove)) {
			updatedParticles.Remove(particleToRemove);
		}
        
        try {
            particleToRemove.QueueFree();
        }
        catch {
            
        }
    }

	public override void _Process(double delta) {
		foreach (var particle in updatedParticles.Keys) {
			particle.Position = updatedParticles[particle].Position; 
		}
	}

	static Dictionary<GpuParticles2D, Node2D> updatedParticles = new();
	public static GpuParticles2D SpawnGlobalFolliwngParticle(PackedScene particle, Node2D nodeToFollow) {
		
		GpuParticles2D instance =  particle.Instantiate<GpuParticles2D>();
		factoryNode.AddChild(instance);

		updatedParticles.Add(instance, nodeToFollow);
		nodeToFollow.TreeExited += () => updatedParticles.Remove(instance);
		
		return instance;
	}

	public static GpuParticles2D SpawnGlobalParticle(GpuParticles2D particle, Vector2 position, float rotation) {
		
		GpuParticles2D newParticles = (GpuParticles2D) particle.Duplicate();
		factoryNode.AddChild(newParticles);
		newParticles.Position = position;
		newParticles.Rotation = rotation;
		newParticles.Emitting = true;
		return newParticles;
	}
}
