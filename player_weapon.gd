extends Node2D
class_name Player

@onready var _gun_animation_tree = $GunAnimationTree
@export var equipt_weapon: Weapon

func _process(delta):
	hide_gun_behind_player()

#In the future, this has to go somewhere else. But for now, it's here. 
func hide_gun_behind_player():
	var facing_direction: Vector2 = -(global_position - get_global_mouse_position()).normalized()
	_gun_animation_tree.set("parameters/blend_position", facing_direction)

var reload_delay = 0.5

var _reloaded = true
#When the thing is held, loop the attack consistently 
func _on_player_attack():
	if not _reloaded:
		#reload() is taking its time.
		return
	
	#Set to false to filter clicks (filtered in the guard clause above)
	_reloaded = false
	#Will stop waiting when reloaded	
	await reload()
	use_weapon()
	_reloaded = true
	pass # Replace with function body.

func reload():  
	var reload_delay_recharge = 0
	#While the recharge isn't the same as reload delay, increase the time
	while reload_delay_recharge < reload_delay:
		await get_tree().create_timer(get_process_delta_time()).timeout
		reload_delay_recharge += get_process_delta_time()
	reload_delay_recharge = 0
	return true

#May be replaced with specific scripts for weopons possibly?
#That way, weopons can have unique functionality
func use_weapon(): 	
	pass
