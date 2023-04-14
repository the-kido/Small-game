extends Node2D

#@onready var _gun_animation_tree = $AnimationTree
@export var debug_weapon: PackedScene

#Signals
signal attack

#Weapon values (TEMP)
var reload_delay = 0.5
var selected_weapon: BaseWeapon

#Debugging
func _ready():
	change_weapon(debug_weapon)

func change_weapon(weapon: PackedScene):
	for child in get_children():
		child.queue_free()

	var new_weapon: BaseWeapon = weapon.instantiate()
	add_child(new_weapon)
	new_weapon.init(self)
	init_new_weapon(new_weapon)
	
func init_new_weapon(weapon: BaseWeapon):
	selected_weapon = weapon
	reload_delay = weapon.reload_speed


var _reloaded = true
#In the future, this has to go somewhere else. But for now, it's here. 
func on_player_weapon_use(attack_inputs):
	if not _reloaded:
		#reload() is taking its time.
		return
	
	#Set to false to filter clicks (filtered in the guard clause above)
	_reloaded = false
	#Will stop waiting when reloaded
	selected_weapon.use_weapon(attack_inputs)
	
	await reload()
	
	#Call the weapon_use function
	
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
	





