shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"
uniform float initialVelocityMultiplier = 100.;
uniform float rotationMultiplier = 4.;
uniform vec2 SpriteSheetSize = vec2(4., 2.);

void start() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	CUSTOM.z = float(INDEX)/(SpriteSheetSize.x * SpriteSheetSize.y);
	vec2 velDir = vec2(
		(rand_from_seed(alt_seed) * 2.) - 1.,
		(rand_from_seed(alt_seed) * 2.) - 1.
	);
	//angular_velocity
	USERDATA1.x = (rand_from_seed(alt_seed) *2.) - 1.;
	USERDATA1.x *= rotationMultiplier;
	// currentRotation
	USERDATA1.y = USERDATA1.x * PI * 2.;
	VELOCITY.xy = velDir * initialVelocityMultiplier;
	COLOR.a = 1.;
	TRANSFORM[3].xy = VELOCITY.xy/(initialVelocityMultiplier/2.);
}

void process() {
	VELOCITY -= normalize(VELOCITY) * length(VELOCITY) * DELTA;
	COLOR.a -= DELTA/LIFETIME;
	
	USERDATA1.y += USERDATA1.x * DELTA;
	
	float c = cos(USERDATA1.y);
	float s = sin(USERDATA1.y);
	TRANSFORM[0].xyz = vec3(c, -s, 0.0);
	TRANSFORM[1].xyz = vec3(s,  c, 0.0);
}
