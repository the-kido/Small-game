extends Control

@export var level = Resource.new()

func _on_button_button_down():
	SceneSwitcher.change_scene_with_path(level.resource_path)
