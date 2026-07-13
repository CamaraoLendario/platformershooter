shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"

float ibuki(uint v)
{
	const uint mult = 0xae3cc725u;
	
	v *= mult;
	v ^= v ^ v >> 13u; 
	v *= mult;
	
	uint r = v;
	r ^= r >> 11u;
	r = (r * r) ^ r;
	
	return float(r) * 2.3283064365386962890625e-10;
}

void start() {
	TRANSFORM = EMISSION_TRANSFORM;
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	//SpawnAngle
	float spawnAngle = rand_from_seed(alt_seed) * PI * 2.;
	
	vec2 spawnPos = vec2(cos(spawnAngle), sin(spawnAngle)) * rand_from_seed(alt_seed);
	TRANSFORM[3].xy += spawnPos * 50.;
	VELOCITY.xy = TRANSFORM[3].xy * 3. * vec2(1.0, 0.8);
	VELOCITY.xy = vec2 (
		clamp(VELOCITY.x, -50., 50.),
		min(50., VELOCITY.y)
	);
	//VELOCITY.x *= 1.8;
	COLOR.a = 1.;
	CUSTOM.z = rand_from_seed(alt_seed);
	
	USERDATA1.xyzw = vec4(
		// PosOffsetX
		0.,
		// PosOffsetY
		0.,
		// transparencyOffset
		rand_from_seed(alt_seed),
		// time
		0.
	);
	COLOR.rgb = vec3(0.75);
}

void process() {
	USERDATA1.w += DELTA/LIFETIME;
	
	VELOCITY.xy -= VELOCITY.xy * (DELTA/10.) * vec2(10., 1.);
	VELOCITY.y -= DELTA * 50.;
	
	float rotation = clamp(VELOCITY.x/100., -0.5, 0.5);
	
	TRANSFORM[3].xy -= USERDATA1.xy;
	//Generate new PosOffset
	float posOffsetAngle = ibuki(INDEX + uint(USERDATA1.w*100.)) * PI * 2.;
	USERDATA1.xy = normalize(vec2(cos(posOffsetAngle), sin(posOffsetAngle)))*max(0., 1.- USERDATA1.w*1.5)*2.;
	TRANSFORM[3].xy += USERDATA1.xy;
	
	float c = cos(-rotation);
	float s = sin(-rotation);
	TRANSFORM[0].xyz = vec3(c, -s, 0.0);
	TRANSFORM[1].xyz = vec3(s,  c, 0.0);
	float newAlpha = (90. - length(vec2(
		TRANSFORM[3].x,
		max(0., TRANSFORM[3].y)
	)))/30.;
	
	COLOR.a = min(1., 2.-(USERDATA1.w*2.) - USERDATA1.z);
	
	if (COLOR.a > 0.)
		COLOR.a = min(COLOR.a, newAlpha);
}
