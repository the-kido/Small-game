extends CharacterBody2D

const MOVE_SPEED = 300.0
const CORNER_CORRECTION_RANGE = 20 #in pixels

@onready var _player_animation_tree = $AnimationTree

func _physics_process(_delta):
	#Find out which way the character is facing.
	control_player_movement()
	play_movement_animations(player_is_moving())
	move_and_slide()

func control_player_movement():
	#Reset velocity as to not slide around
	velocity = Vector2.ZERO
	
	#Get the direction the character moves via input
	var direction = Vector2(
		Input.get_axis("left", "right"), 
		Input.get_axis("up", "down")
		).normalized()
	
	if direction != Vector2.ZERO:
		corner_correction(direction)
		velocity = direction * MOVE_SPEED

#https://www.youtube.com/watch?v=tW-Nxbxg5qs
#Thanks. That Celeste code had me like @_@
func corner_correction(movement_direction: Vector2):
	#Check if there is an obstacle above which can be avoided
	if test_move(global_transform, Vector2(0, movement_direction.y)):
		
		#Find where exactly this object's corner is offseted. 
		#The range (20 — -20) accounts for both left and right shifting
		for x_offset in range(CORNER_CORRECTION_RANGE, -CORNER_CORRECTION_RANGE -1, -5):
			#Check again if the player is in the way of an obstacle
			#If so, then continue along. If not, then shift. 
			if test_move(
				global_transform.translated(Vector2(x_offset, 0)), 
				Vector2(0, movement_direction.y)
				):
					continue
			translate(Vector2(x_offset / 1.5, 0))
			return
	#Same thing, but for x
	elif test_move(global_transform, Vector2(movement_direction.x, 0)):
		#Find where exactly this object's corner is offseted 
		for y_offset in range(CORNER_CORRECTION_RANGE, -CORNER_CORRECTION_RANGE -1, -5):
			if test_move(
				global_transform.translated(Vector2(0, y_offset)), 
				Vector2(movement_direction.x, 0)
				): continue

			translate(Vector2(0, y_offset / 1.5))
			return

#Used to check for movement of the player for the player_is_moving() function
var previous_frame_position = Vector2()
func player_is_moving() -> bool:
	if position == previous_frame_position:
		previous_frame_position = position
		return false
	previous_frame_position = position
	return true


func play_movement_animations(is_moving: bool):
	
	var player_direction_normal = -(position - get_global_mouse_position()).normalized()
	
	_player_animation_tree.set("parameters/conditions/idle", !is_moving)
	_player_animation_tree.set("parameters/conditions/walk", is_moving)
	
	_player_animation_tree.set("parameters/Walk/blend_position", player_direction_normal)
	_player_animation_tree.set("parameters/Idle/blend_position", player_direction_normal)

