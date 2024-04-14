shader_type canvas_item;

uniform float starting = 0.0;
uniform vec2 r_displacement = vec2(3.0, 0.0);
uniform vec2 g_displacement = vec2(0.0, 0.0);
uniform vec2 b_displacement = vec2(-3.0, 0.0);

uniform sampler2D screen_texture : hint_screen_texture;
 
float rand(vec2 co){
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

void fragment()
{
	// Ranges from [0.5, 1]
	float anim = (0.125 * sin(5.0 * TIME) + 0.75) * starting;
	
	vec2 mag = SCREEN_UV.xy - vec2(0.5);
	vec2 suv = (SCREEN_UV) + (mag*mag*mag * -0.5) * anim;
	
	vec2 displace = (suv - vec2(0.5)) * anim + 0.25 * rand(vec2(TIME));
	
	float redFactor = (1.0 - starting) + (starting * (1.0 - pow(length(mag), 2.0)));

	float r = texture(screen_texture, suv + vec2(SCREEN_PIXEL_SIZE*r_displacement * displace), 0.0).r;
	float g = redFactor * texture(screen_texture, suv + vec2(SCREEN_PIXEL_SIZE*g_displacement * displace), 0.0).g;
	float b = redFactor * texture(screen_texture, suv + vec2(SCREEN_PIXEL_SIZE*b_displacement * displace), 0.0).b;
	
	COLOR = vec4(r, g, b, 1.0);
	
	
	
}