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

	public static Node2D AddParticleToNode(Node2D node, PackedScene particle) {
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
	
	public static void AddFollwingParticleToFactory(GpuParticles2D particle, Node2D followedNode) {
		followedNode.TreeExiting += () => {
        	UpdatedParticles.Remove(particle);
			RemoveParticle(particle);
		};

        UpdatedParticles.Add(particle, followedNode);

		particle.Reparent(factoryNode, true);
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
