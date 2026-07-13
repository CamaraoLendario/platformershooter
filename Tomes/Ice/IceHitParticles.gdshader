shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"
uniform vec2 direction = vec2(-1., 0.);
uniform float coneAngle = 1.;
uniform int particlesCount = 7;

void start() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	
	TRANSFORM[3].xy *= 0.;
	VELOCITY.xy = RotateVec(direction, ((rand_from_seed(alt_seed)-0.5)*2.) * coneAngle) * 300. * rand_from_seed(alt_seed);
	CUSTOM.z = float(INDEX)/float(particlesCount);
	USERDATA1.x = 0.;
}

void process() {
	USERDATA1.x += DELTA * 3.;
	VELOCITY.xy -= VELOCITY.xy * DELTA * 10.;
	
	COLOR.a = 1.5-USERDATA1.x;
}
