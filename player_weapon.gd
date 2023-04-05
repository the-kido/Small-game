extends Node2D

@onready var _gun_animation_tree = $GunAnimationTree

func _process(delta):
	hide_gun_behind_player()

#In the future, this has to go somewhere else. But for now, it's here. 
func hide_gun_behind_player():
	var facing_direction: Vector2 = -(global_position - get_global_mouse_position()).normalized()
	_gun_animation_tree.set("parameters/blend_position", facing_direction)

