class_name BaseWeapon
extends Node2D

#Only mention variables relavent to other scripts (i.e reload speed for
#reload() in player_weapon)
@export var reload_speed: float

@onready var animation_player: AnimationTree = $AnimationTree

var _parent_node = get_parent()

func use_weapon(attack_inputs: Array[String]):
	push_error("use_weapon() from base class BaseWeapon was not implemented for " + name)

func _hide_weapon_behind_player():
	var facing_direction: Vector2 = -(global_position - get_global_mouse_position()).normalized()
	animation_player.set("parameters/blend_position", facing_direction)


func init(weapon_holder: Node2D):
	_parent_node = weapon_holder
	

func face_weapon_to_cursor():
	_parent_node.look_at(get_global_mouse_position())


func _process(_delta):
	#_hide_weapon_behind_player()
	face_weapon_to_cursor()
	
	
