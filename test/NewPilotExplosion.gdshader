shader_type particles;
uniform vec2 initialDir = vec2(1.);
uniform sampler2D mainTexture;
uniform vec3 outlineColor: source_color = vec3(1., 0., 0.);

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

vec2 RotateVec(vec2 inputVec, float angle){
	vec2 outputVec;
	vec2 v1 = vec2(cos(angle), sin(angle));
	vec2 v2 = vec2(-sin(angle), cos(angle));
	outputVec = inputVec.x*v1 + inputVec.y*v2;
	return outputVec;
}
vec4 getOutlines(vec2 texUV, vec4 currentColor){
	if (currentColor.a != 0.) return currentColor;
	
	vec2 quadrant = (ceil((texUV*2.)-vec2(1.))*2.)-vec2(1.);
	vec2 Hquadrant = quadrant * vec2(1, 0);
	vec2 Vquadrant = quadrant * vec2(0, 1);
	vec2 dirs[4] = {
		-Hquadrant,
		-Vquadrant,
		Hquadrant,
		Vquadrant
		};
	for (int i = 0; i < 4; i++){
		vec2 HUVoffset = dirs[i] / vec2(textureSize(mainTexture, 0));
		float alpha = texture(mainTexture, texUV + HUVoffset).a;
		if (alpha != 0.) return vec4(outlineColor, 1.);
	}
	return vec4(0.);
}

void start() {
	ivec2 texSize = textureSize(mainTexture, 0);
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	vec2 pixelCoord = vec2(ivec2(
		int(INDEX) % texSize.x,
		int(INDEX) / texSize.x
	));
	TRANSFORM[3].xy = pixelCoord - (vec2(texSize) / 2.);
	vec2 texUV = (pixelCoord + vec2(.5)) / vec2(texSize);
	
	COLOR = texture(mainTexture, texUV);
	COLOR = getOutlines(texUV, COLOR);
	VELOCITY.xy = (RotateVec(TRANSFORM[3].xy, (PI/2.) * ((rand_from_seed(alt_seed)-.5)*2.))*3.) * (1.+rand_from_seed(alt_seed));
	VELOCITY.xy += initialDir*30.;
	
	TRANSFORM[3].xy += VELOCITY.xy/8.;
	
	//VELOCITY.xy *= 0.;
	USERDATA1.xy = normalize(TRANSFORM[3].xy);
}

void process() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	vec2 acceleration = (-TRANSFORM[3].xy * DELTA)/length(TRANSFORM[3].xy);
	
	VELOCITY.xy += acceleration*100.;
	
	COLOR.a = min(COLOR.a, dot(normalize(TRANSFORM[3].xy), USERDATA1.xy));
	
	vec2 normalizedVel = normalize(VELOCITY.xy);
	float currentRotation = atan(normalizedVel.y/normalizedVel.x);// * value;
	float c = cos(-currentRotation);
	float s = sin(-currentRotation);
	TRANSFORM[0].xyz = vec3(c, -s, 0.0);
	TRANSFORM[1].xyz = vec3(s,  c, 0.0);
}
