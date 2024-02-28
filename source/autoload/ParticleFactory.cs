using Game.Graphics;
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

	// Key: Instances
	// Value: The effect that it's associated with. 
    static readonly Dictionary<Node2D, Effect> instancedEffects = new();
	// particle | followed
	static readonly Dictionary<Node2D, Node2D> UpdatedParticles = new();
	
	static Rect2 visiblityRect = new(-2000, -2000, new(4000,4000));

	private static Node2D InstanceEffect(Effect particle, Node parent) {
        Node2D instance = (Node2D) particle.packedScene.Instantiate();
		instancedEffects.Add(instance, particle);

		// This might be performance heavy so create a warning to make the rect a better size in the future (maybe)?
		if (instance is GpuParticles2D gpuParticles2D) gpuParticles2D.VisibilityRect = visiblityRect; 

		parent.AddChild(instance);

		return instance;
	}

	public static Node2D AddParticleToNode(Node2D node, Effect particle) {
		var instance = InstanceEffect(particle, node);
        return instance;
    }

	public static void AddFollowingParticle(Effect effect, Node2D followedNode) {
		Node2D instance = InstanceEffect(effect, factoryNode);

		followedNode.TreeExiting += () => {
        	UpdatedParticles.Remove(instance);
			RemoveEffect(instance);
		};

        UpdatedParticles.Add(instance, followedNode);
	}

	public static void AddInstantParticle(Effect effect, Vector2 position, float rotation) {
		Node2D newParticle = InstanceEffect(effect, factoryNode);
		
		if (newParticle is GpuParticles2D particles) particles.Emitting = true;
		
		newParticle.Position = position;
		newParticle.Rotation = rotation;
		RemoveEffect(newParticle);
	}

	// Removes particle after 5 seconds.
	public async static void RemoveEffect(Node2D effect) {
		instancedEffects[effect].removeAnimation(effect);
		instancedEffects.Remove(effect);
		
		// Here to avoid errors during the 5 seconds that the particle could possibly be queue free'd	
		bool queueFreed = false;
		effect.TreeExited += () => queueFreed = true;

        // 5 seconds is p safe time to let all the particles go away before deleted the instance.
        await Task.Delay(5000);
		
		// Stop updating this particle's position. 
		if (UpdatedParticles.ContainsKey(effect)) UpdatedParticles.Remove(effect);

		if (queueFreed is not true) effect.QueueFree();
    }

	public override void _PhysicsProcess(double delta) {
		foreach (Node2D particle in UpdatedParticles.Keys) particle.Position = UpdatedParticles[particle].Position; 
	}
}
