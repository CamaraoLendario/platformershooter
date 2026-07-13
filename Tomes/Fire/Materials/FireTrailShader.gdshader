shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"

void start() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	TRANSFORM[3].xy = EMISSION_TRANSFORM[3].xy;
	float angle = rand_from_seed(alt_seed) * PI * 2.;
	TRANSFORM[3].xy += vec2(cos(angle), sin(angle)) * 2.;// * rand_from_seed(alt_seed);
	VELOCITY.xy *= 0.;
	//TIME
	USERDATA1.x = 0.;
	// randAngle
	USERDATA1.y = rand_from_seed(alt_seed) * PI * 2.;
	
	float c = cos(USERDATA1.y);
	float s = sin(USERDATA1.y);
	TRANSFORM[0].xyz = vec3(c, -s, 0.0);
	TRANSFORM[1].xyz = vec3(s,  c, 0.0);
}

void process() {
	USERDATA1.x += DELTA/LIFETIME;
	VELOCITY.y -= DELTA * 100.;
	CUSTOM.z = 0.15 + USERDATA1.x*0.85;
	COLOR.a = max(0., 1. - USERDATA1.x*1.);
}
