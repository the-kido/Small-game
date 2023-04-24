using Godot;
using System;

public partial class ParticleFactory : Node
{
	// Called when the node enters the scene tree for the first time.
	private static Node factoryNode;
	public override void _Ready() {
		factoryNode = GetNode<ParticleFactory>("/root/ParticleFactory");
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
