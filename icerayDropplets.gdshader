shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"

uniform float dist = 30.;
uniform vec3 color : source_color = vec3(1.);
uniform float shakeTime = 0.3;
uniform float fallDelay = 0.0;
uniform vec2 dir = vec2(1., 0.);

vec2 shake(uint alt_seed, vec4 userData1){
	float angle = rand_from_seed(alt_seed) * PI * 2.;
	vec2 shakeOffset = vec2(cos(angle), sin(angle)) * 2.;
	shakeOffset *= 1. - min(1., userData1.w / shakeTime);
	return userData1.xy + shakeOffset;
}

void start() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	
	USERDATA1 *= 0.; // XY - position | W - TIME
	VELOCITY *= 0.;
	
	USERDATA1.xy = normalize(dir) * dist * (float(INDEX) / (dist/5.));
	float angle = rand_from_seed(alt_seed) * PI * 2.;
	vec2 spawnOffset = vec2(cos(angle), sin(angle)) * 2.;
	spawnOffset.y /= 2.;
	USERDATA1.xy += spawnOffset;
	CUSTOM.z = rand_from_seed(alt_seed); // sprite chosen in spriteSheet 0-1
	COLOR.rgb = color;
	COLOR.a = 1.;
}

void process() {
	uint alt_seed = hash(NUMBER + uint(1.+TIME*60.) + RANDOM_SEED);
	USERDATA1.w += DELTA;
	COLOR.a = smoothstep(1., 1., USERDATA1.w + 1. - shakeTime * (float(INDEX) / (dist/5.))*0.2);
	if (USERDATA1.w <= shakeTime)
		TRANSFORM[3].xy = shake(alt_seed, USERDATA1);
	else if (USERDATA1.w - (float(INDEX)*5.)/dist > shakeTime + fallDelay){
		VELOCITY.y += 50. * DELTA;
		COLOR.a = 1. - VELOCITY.y/20.;
	}
}
