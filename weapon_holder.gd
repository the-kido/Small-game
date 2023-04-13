extends Node2D

@onready var _gun_animation_tree = $GunAnimationTree

#Signals
signal attack

#Weapon values
var reload_delay = 0.5

func change_weapon(weapon: BaseWeapon):
	pass

'''
THIS IS DEBUGGING. DEFAUTL THIS TO SOME SORT OF "NEW LEVEL" SYSTEM TO START
WITH A DEFAULT WEAPON
'''
@export var default_weapon: BaseWeapon
#func _ready():
#	change_weapon("")



var _reloaded = true
#In the future, this has to go somewhere else. But for now, it's here. 
func on_player_weapon_use(attack_inputs):
	if not _reloaded:
		#reload() is taking its time.
		return
	
	#Set to false to filter clicks (filtered in the guard clause above)
	_reloaded = false
	#Will stop waiting when reloaded	
	print(attack_inputs)
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
	





