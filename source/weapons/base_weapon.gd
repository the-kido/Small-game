extends Node2D
class_name BaseWeapon

#Only mention variables relavent to other scripts (i.e reload speed for
#reload() in player_weapon)
@export var reload_speed: int
@export var reload_delay: float
@onready var animation_player: AnimationTree = $AnimationTree


func use_weapon():
	push_error("use_weapon() from base class BaseWeapon was not implemented for " + name)


func _process(_delta):
	_hide_weapon_behind_player()
	
func _hide_weapon_behind_player():
	var facing_direction: Vector2 = -(global_position - get_global_mouse_position()).normalized()
	animation_player.set("parameters/blend_position", facing_direction)
