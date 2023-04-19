extends Node2D

func spawn_bullet(bullet_prototype: PackedScene) -> BaseBullet:
	var new_bullet: BaseBullet = bullet_prototype.instantiate()
	add_child(new_bullet)
	
	if not new_bullet is BaseBullet:
		push_error("The PackedScene " + bullet_prototype.resource_path + " was not of type BaseBullet")
		return null
	return new_bullet
	
 
