shader_type canvas_item;

//defines the coefficient
uniform float fancy;
uniform int powa = 3;
uniform float tetsa;
uniform float tetsa2;
uniform sampler2D screen_texture : hint_screen_texture;

vec2 powerGood(vec2 vec, float powerVal) {
	return vec2( pow(vec.x, powerVal), pow(vec.y, powerVal));
}

void fragment(){
	vec2 mag = SCREEN_UV.xy - vec2(0.5);
	vec2 suv = (SCREEN_UV) + (powerGood(mag, 3.0) * fancy) ;
	COLOR = texture(screen_texture, suv);
	
	if (length(mag) >= tetsa) {
		COLOR.r = 1.0;
	}
		//
	//if (length(dir) >= tetsa2) {
		//COLOR.b = 1.0;
	//}
}