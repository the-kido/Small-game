extends Node2D
class_name BaseBullet

@export var damage: int
@export_range(0, 1000) var speed: int
@onready var particle := $GPUParticles2D
@onready var area_2d:= $Area2D

signal OnBulletCollision(bullet: BaseBullet, body: Node2D)

func _on_area_2d_body_entered(body: Node2D):
	OnBulletCollision.emit(self, body)

func destroy():
	ParticleFactory.spawn_global_particle(particle, global_position, global_rotation+90)
	queue_free()

var _direction: Vector2
func init(spawn_position: Vector2, nuzzle_rotation: float):
	rotation = nuzzle_rotation
	_direction = Vector2(cos(nuzzle_rotation), sin(nuzzle_rotation))
	position = spawn_position
	
func _process(delta):
	position += _direction * delta * speed
	pass
