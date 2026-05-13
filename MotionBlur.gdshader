shader_type canvas_item;

uniform vec2 blurDir = vec2(-1, 0);
uniform float value;

uint hash(uint x) {
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = (x >> uint(16)) ^ x;
	return x;
}
float GetRand(float offset){
	return fract(float(hash(uint(offset * TIME)))/10000.);
}
void vertex() {
	
}

void fragment() {
}
