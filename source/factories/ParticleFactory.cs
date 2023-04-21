using Godot;
using System;

public partial class ParticleFactory : Node
{
	// Called when the node enters the scene tree for the first time.
	public GpuParticles2D SpawnGlobalParticle(GpuParticles2D particle, Vector2 position, float rotation) {
		GpuParticles2D newParticles = (GpuParticles2D) particle.Duplicate();
		AddChild(newParticles);
		newParticles.Position = position;
		newParticles.Rotation = rotation;
		newParticles.Emitting = true;
		return newParticles;
	}
}
