using Godot;
using KidoUtils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Autoload;


public partial class ParticleFactory : Node {
	// Called when the node enters the scene tree for the first time.
	private static Node factoryNode;
	public override void _Ready() {
		factoryNode = Utils.GetPreloadedScene<ParticleFactory>(this, PreloadedScene.ParticleFactory);
	}

	public static Node2D AddParticle(Node2D node, PackedScene particle) {
        Node2D instance = (Node2D) particle.Instantiate();
        node.AddChild(instance);
        return instance;
    }
	public async static void RemoveParticle(Node2D particleToRemove) {
		if (particleToRemove is GpuParticles2D gpuParticles3D) {
        	gpuParticles3D.Emitting = false;

		}
        
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

	static Dictionary<Node2D, Node2D> updatedParticles = new();
	public static Node2D SpawnGlobalFolliwngParticle(PackedScene particle, Node2D nodeToFollow) {
		
		Node2D instance =  particle.Instantiate<Node2D>();
		factoryNode.AddChild(instance);

		updatedParticles.Add(instance, nodeToFollow);
		nodeToFollow.TreeExited += () => updatedParticles.Remove(instance);
		
		return instance;
	}

	public static Node2D SpawnGlobalParticle(Node2D particle, Vector2 position, float rotation) {
		
		Node2D newParticles = (Node2D) particle.Duplicate();
		factoryNode.AddChild(newParticles);
		newParticles.Position = position;
		newParticles.Rotation = rotation;
		if (particle is GpuParticles2D gpuParticles2D)
			gpuParticles2D.Emitting = true;
		return newParticles;
	}
}
