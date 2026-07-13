shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"

uniform vec3[3] colors : source_color;

void start() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	//SpawnAngle
	USERDATA1.y = rand_from_seed(alt_seed) * PI * 2.;
	
	vec2 spawnPos = vec2(cos(USERDATA1.y), sin(USERDATA1.y)) * 5. * rand_from_seed(alt_seed);
	TRANSFORM[3].xy = spawnPos;
	VELOCITY.xy = normalize(spawnPos) * ((rand_from_seed(alt_seed)*.75) + 0.25) * 100.;
	//Time
	USERDATA1.x = 0.;
	//lifetime randomizer. makes them disapear earlier or later
	USERDATA1.z = rand_from_seed(alt_seed) * 0.5;
	COLOR.rgb = colors[int(rand_from_seed(alt_seed) * 2.99999999)] * 1.8;
	COLOR.a = 1.;
	
	float c = cos(-USERDATA1.y);
	float s = sin(-USERDATA1.y);
	TRANSFORM[0].xyz = vec3(c, -s, 0.0);
	TRANSFORM[1].xyz = vec3(s,  c, 0.0);
}

void process() {
	VELOCITY.xy -= VELOCITY.xy * DELTA * 2.;
	USERDATA1.x += (DELTA/LIFETIME) * 3.;
	COLOR.a = min(1., 3. - USERDATA1.x - (USERDATA1.z*2.));
}
