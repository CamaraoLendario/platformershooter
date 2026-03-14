shader_type particles;

uniform float radiusLength = 50.;
uniform float initialVelocity = 100.;
uniform float pullStrength = 100.;

uniform vec4 color1 :source_color;
uniform vec4 color2 :source_color;
uniform vec4 color3 :source_color;


vec2 RotateVec(vec2 inputVec, float angle){
	vec2 outputVec;
	vec2 v1 = vec2(cos(angle), sin(angle));
	vec2 v2 = vec2(-sin(angle), cos(angle));
	outputVec = inputVec.x*v1 + inputVec.y*v2;
	return outputVec;
}

uint hash(uint x) {
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = (x >> uint(16)) ^ x;
	return x;
}

float rand_from_seed(inout uint seed) {
	int k;
	int s = int(seed);
	if (s == 0) {
		s = 305420679;
	}
	k = s / 127773;
	s = 16807 * (s - k * 127773) - 2836 * k;
	if (s < 0) {
		s += 2147483647;
	}
	seed = uint(s);
	return float(seed % uint(65536)) / 65535.0;
}

void start() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	float x = rand_from_seed(alt_seed) - .5;
	float y = rand_from_seed(alt_seed) - .5;
	TRANSFORM[3].xy = normalize(vec2(x, y)) * (radiusLength + rand_from_seed(alt_seed) * 20.); 
	VELOCITY.xy = RotateVec(normalize(TRANSFORM[3].xy), PI/1.2) * initialVelocity;
	CUSTOM.xy = TRANSFORM[3].xy;
	CUSTOM.w = length(TRANSFORM[3].xy);
	COLOR.a = 1.;
	CUSTOM.z = 0.;
}

void process() {
	float normalizedDistanceToCenter = length(TRANSFORM[3].xy) / CUSTOM.w;
	CUSTOM.z += DELTA/LIFETIME;
	// TODO: explore this VV
	//VELOCITY.xy -= RotateVec(normalize(TRANSFORM[3].xy), PI/2.) * 6.;
	// VV normal friction VV
	VELOCITY.xy -= VELOCITY.xy * DELTA * 4.;
	VELOCITY.xy += -normalize(TRANSFORM[3].xy) * pullStrength * DELTA * (1. - normalizedDistanceToCenter);
	
	normalizedDistanceToCenter = smoothstep(0, 1, normalizedDistanceToCenter);
	if (normalizedDistanceToCenter > 0.5)
		COLOR = mix(color2, color1, (normalizedDistanceToCenter*2.) - 1.);
	else
		COLOR = mix(color3, color2, (normalizedDistanceToCenter*2.));
	
	COLOR.a = min(CUSTOM.z*3., 1);
	//if (dot(normalize(CUSTOM.xy), normalize(TRANSFORM[3].xy)) <= -0.4){
		//COLOR.a = 0.;
	//}
}
