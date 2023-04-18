extends Node

signal weapon_use(attack_inputs)

 
var _filter_all_input = false
func toggle_player_input(toggle: bool):
	_filter_all_input = toggle
	
func detect_attack_input():
	var attack_inputs: Dictionary = {}
	
	if Input.is_action_pressed("default_attack"):
		attack_inputs[attack_inputs.size()+1] = "Left Click"
	
	if attack_inputs.size() != 0:
		weapon_use.emit(attack_inputs)
		
	
func _process(_delta):
	if _filter_all_input:
		return
	
	detect_attack_input()
