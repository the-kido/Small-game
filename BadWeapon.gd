extends BaseWeapon

@onready var bullet: BaseBullet = preload("res://source/weapons/bullets/base_bullet.tscn").instantiate()
@onready var nuzzle: Node2D = $Nuzzle

func use_weapon(attack_inputs: Dictionary):
	print(bullet.damage)
	pass
