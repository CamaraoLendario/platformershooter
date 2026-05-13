shader_type canvas_item;

uniform float flashPerSec = 1;
uniform float maxAlpha = 1;

void vertex() {
	COLOR.a = min(1., (sin(TIME*PI*2.*flashPerSec)+1.))*maxAlpha;
}
