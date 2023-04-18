extends RigidBody2D
class_name Entity


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func color_pulsate(duration: float, color: Color):
	pulse_color = color
	pulse_duration = duration
	pulse_time = 0
	
var pulse_color = Color.WHITE
var pulse_duration = 0
var pulse_time = 1
func update_color_pulse(delta):
	if pulse_time > pulse_duration:
		return
	pulse_time += delta
	modulate = lerp(pulse_color, Color(1,1,1,1), pulse_time/pulse_duration)

	#if pulse_time > pulse_duration:
	#	pulse_color = Color.WHITE
	#	pulse_duration = 0
	#	pulse_time = 10000
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	update_color_pulse(delta)
