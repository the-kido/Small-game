extends RigidBody2D
class_name BaseEnemy

@export var max_health: int
@onready var sprite := $Sprite
var _health: int

func _ready():
	_health = max_health

#Replace w/ decorator pattern if necessary.
func inflict_damage(damage: int):
	_health -= damage
	if _health <= 0:
		kill()

func kill():
	queue_free()
	pass

func _process(delta):
	pass
