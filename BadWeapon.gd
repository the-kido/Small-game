extends BaseWeapon

@onready var bullet_asset = preload("res://source/weapons/bullets/base_bullet.tscn")
@onready var nuzzle: Node2D = $Nuzzle

func use_weapon(attack_inputs: Dictionary):
	
	var new_bullet = BulletFactory.spawn_bullet(bullet_asset)
	new_bullet.init(nuzzle.global_position, nuzzle.global_rotation)
	#why is this hurt brain
	new_bullet.OnBulletCollision.connect(on_hit)
	
func on_hit(bullet: BaseBullet, body: Node2D):
	if body is BaseEnemy:
		#body is therefor an enemy
		var enemy: BaseEnemy = body
		enemy.inflict_damage(bullet.damage)
	
	bullet.destroy()
