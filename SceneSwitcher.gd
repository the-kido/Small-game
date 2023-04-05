extends Node

#https://www.youtube.com/watch?v=yZQStB6nHuI
@onready var animation_player = $AnimationPlayer


func _change(action):
	#Put this above all else (minus space for errors while loading)
	$CanvasLayer.layer = 127
	animation_player.play("panel_fade")
	await $AnimationPlayer.animation_finished
	action.call()
	animation_player.play_backwards("panel_fade")
	await $AnimationPlayer.animation_finished
	#Hide the layer behind everything else as to not be a disturbance. 
	$CanvasLayer.layer = -128

func change_scene_with_path(scene):
	_change(
		func(): get_tree().change_scene_to_file(scene) 
	)

func change_scene_with_packed_map(packed_scene):
	_change(
		func(): get_tree().change_scene_to_packed(packed_scene)
	)
	
