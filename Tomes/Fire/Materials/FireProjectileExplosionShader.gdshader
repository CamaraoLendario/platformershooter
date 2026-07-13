shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"
uniform float tendrilEveners = 8;
uniform int layers = 4;

void start() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	
	TRANSFORM = EMISSION_TRANSFORM;
	
	float angle = (float(INDEX)/tendrilEveners) * PI * 2.;
	vec2 posOffset = cossin(angle) * 6. * float(int(INDEX)%layers+1);
	posOffset += cossin(rand_from_seed(alt_seed)*PI*2.)*3.;
	 
	TRANSFORM[3].xy += posOffset;
	// float angle = (float(INDEX)/float(tendrilEveners) + rand_from_seed(alt_seed)/3.) * PI * 2.;
	// TRANSFORM[3].xy += vec2(cos(angle), sin(angle)) * 25. * rand_from_seed(alt_seed);
	VELOCITY.xy = posOffset * 3.;
	// TIME
	USERDATA1.w = 0.0;
	// LIFE Rand
	USERDATA1.z = rand_from_seed(alt_seed);
	// initialRotation
	USERDATA1.y = rand_from_seed(alt_seed)*PI*2.;
	
	VELOCITY.y -= USERDATA1.z * 10.;
}

void process() {
	USERDATA1.w += min(1., (DELTA/LIFETIME) * (1. + (USERDATA1.z*0.75)));
	VELOCITY.xy -= VELOCITY.xy * DELTA * 1.;
	VELOCITY.y -= DELTA*50. * (1. + USERDATA1.z*2.);
	CUSTOM.z = USERDATA1.w;
	COLOR.a = (2.- USERDATA1.w * 2.);
	
	USERDATA1.y += DELTA * ((USERDATA1.z)*2. -1.) * 6.;
	
	float c = cos(USERDATA1.y);
	float s = sin(USERDATA1.y);
	TRANSFORM[0].xyz = vec3(c, -s, 0.0);
	TRANSFORM[1].xyz = vec3(s,  c, 0.0);
}
