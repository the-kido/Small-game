extends Node

func spawn_global_particle(particle: GPUParticles2D, position: Vector2, angle: float):
	var new := particle.duplicate()
	add_child(new)
	new.position = position
	new.rotation = angle
	new.set_emitting(true)
