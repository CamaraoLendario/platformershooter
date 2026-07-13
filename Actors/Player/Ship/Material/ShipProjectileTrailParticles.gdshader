shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"
uniform vec2 projectileDir = vec2(1., 0); 
uniform vec3[3] colors : source_color;

void start() {
	TRANSFORM = EMISSION_TRANSFORM;
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	//SpawnAngle
	float spawnAngle = rand_from_seed(alt_seed) * PI * 2.;
	
	vec2 spawnPos = vec2(cos(spawnAngle), sin(spawnAngle)) * 3. * rand_from_seed(alt_seed);
	TRANSFORM[3].xy += spawnPos;
	VELOCITY.xy = -RotateVec(projectileDir, (rand_from_seed(alt_seed)*2. -1.) * PI * 0.05) * (5. + 2.5* rand_from_seed(alt_seed));
	
	COLOR.rgb = colors[int(rand_from_seed(alt_seed) * 2.99999999)] * 1.8;
	
	float velAngle = atan(VELOCITY.x/VELOCITY.y);
	float c = cos(velAngle);
	float s = sin(velAngle);
	TRANSFORM[0].xyz = vec3(c, -s, 0.0);
	TRANSFORM[1].xyz = vec3(s,  c, 0.0);
	USERDATA1.x = 0.;
}

void process() {
	USERDATA1.x += (DELTA/LIFETIME) * 3.;
	COLOR.a = min(1., 3. - USERDATA1.x - (USERDATA1.z*2.));
}
