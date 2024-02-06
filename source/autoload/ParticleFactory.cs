using Godot;
using KidoUtils;
using System.Collections.Generic;
using System.Security.Principal;
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
		if (particleToRemove is GpuParticles2D gpuParticles) gpuParticles.Emitting = false;

		// Here to avoid errors during the 5 seconds that the particle could possibly be queue free'd	
		bool queueFreed = false;
		particleToRemove.TreeExited += () => queueFreed = true;

        // 5 seconds is p safe time to let all the particles go away before deleted the instance.
        await Task.Delay(5000);
		
		// Stop updating this particle's position. 
		if (UpdatedParticles.ContainsKey(particleToRemove)) UpdatedParticles.Remove(particleToRemove);

		if (queueFreed is not true) particleToRemove.QueueFree();
    }

	public override void _PhysicsProcess(double delta) {
		foreach (Node2D particle in UpdatedParticles.Keys) particle.Position = UpdatedParticles[particle].Position; 
	}

	static readonly Dictionary<Node2D, Node2D> UpdatedParticles = new();
	
	public static Node2D SpawnGlobalFolliwngParticle(PackedScene particle, Node2D followedNode) {
        Node2D particleInstance = particle.Instantiate<Node2D>();

        UpdatedParticles.Add(particleInstance, followedNode);

        // Remove particle if node it's following is destroyed
        followedNode.TreeExited += () => UpdatedParticles.Remove(particleInstance);
        factoryNode.AddChild(particleInstance);

		// Set the position on a deferred call because otherwise it shows 
		// a bit of it at the coords 0,0 and I DON'T KNOW WHY.
        factoryNode.CallDeferred("SetParticlePosition", followedNode, particleInstance);

        return particleInstance;
    }

    private static void SetParticlePosition(Node2D followedNode, Node2D particleInstance) {
        particleInstance.GlobalPosition = followedNode.GlobalPosition;
    }

	public static Node2D SpawnGlobalParticle(GpuParticles2D particle, Vector2 position, float rotation) {
		GpuParticles2D newParticle = (GpuParticles2D) particle.Duplicate();
		factoryNode.AddChild(newParticle);

		newParticle.Position = position;
		newParticle.Rotation = rotation;
		newParticle.Emitting = true;
		
		return newParticle;
	}
}
