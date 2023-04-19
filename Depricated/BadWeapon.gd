extends BaseWeapon

@onready var bullet_asset = preload("res://source/weapons/bullets/base_bullet.tscn")
@onready var nuzzle: Node2D = $Nuzzle

func use_weapon(attack_inputs: Array[String]):
	
	var new_bullet = BulletFactory.spawn_bullet(bullet_asset)
	new_bullet.init(nuzzle.global_position, nuzzle.global_rotation)
	
