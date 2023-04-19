class_name BaseWeapon
extends Node2D

#Only mention variables relavent to other scripts (i.e reload speed for
#reload() in player_weapon)
@export var reload_speed: float
@export var initial_weapon: PackedScene

@onready var animation_player: AnimationTree = $AnimationTree

@onready var _parent_node = get_parent()


func _ready():
	change_weapon(initial_weapon)

func use_weapon(attack_inputs: Array[String]):
	push_error("use_weapon() from base class BaseWeapon was not implemented for " + name)

func _hide_weapon_behind_player():
	var facing_direction: Vector2 = -(global_position - get_global_mouse_position()).normalized()
	animation_player.set("parameters/blend_position", facing_direction)

func init(weapon_holder: Node2D):
	pass


func face_weapon_to_cursor():
	_parent_node.look_at(get_global_mouse_position())


func _process(_delta):
	#_hide_weapon_behind_player()
	face_weapon_to_cursor()
	

func change_weapon(weapon: PackedScene):
	for child in get_children():
		child.queue_free()

	var new_weapon: BaseWeapon = weapon.instantiate()
	add_child(new_weapon)
	new_weapon.init(self)
	_parent_node = get_parent()
	
	
	#$"../Input Controller".connect("UseWeapon", on_player_weapon_use)
	
var _reloaded = true
func on_player_weapon_use(attack_inputs):
	if not _reloaded:
		#reload() is taking its time.
		return
	
	#Set to false to filter clicks (filtered in the guard clause above)
	_reloaded = false
	#Will stop waiting when reloaded
	use_weapon(attack_inputs)
	
	await reload()
	
	#Call the weapon_use function
	
	_reloaded = true
	pass # Replace with function body.


func reload():  
	var reload_delay_recharge = 0
	#While the recharge isn't the same as reload delay, increase the time
	while reload_delay_recharge < reload_speed:
		await get_tree().create_timer(get_process_delta_time()).timeout
		reload_delay_recharge += get_process_delta_time()
	reload_delay_recharge = 0
	return true
	

