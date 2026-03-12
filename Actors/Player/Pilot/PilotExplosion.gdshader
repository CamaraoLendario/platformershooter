shader_type particles;

uniform vec2 initialDir = vec2(1.);
uniform sampler2D mainTexture;
uniform vec3 outlineColor: source_color = vec3(1., 0., 0.);
uniform float rotation;

vec4 getOutlines(vec2 pos){
	vec4 selfColor = texture(mainTexture, pos); 
	if (selfColor.a != 0.) return selfColor;
	
	for(int y = -1; y <= 1; y++){
		for(int x = -1; x <= 1; x++){
			if ((x + y)%2 == 0) continue;
			if (texture(mainTexture, pos + vec2(float(x), float(y))/vec2(16., 17.)).a != 0.){
				return vec4(outlineColor, 1.);
			}
		}
	}
	return vec4(0.);
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

vec2 RotateVec(vec2 inputVec, float angle){
	vec2 outputVec;
	vec2 v1 = vec2(cos(angle), sin(angle));
	vec2 v2 = vec2(-sin(angle), cos(angle));
	outputVec = inputVec.x*v1 + inputVec.y*v2;
	return outputVec;
}

void start() {
	vec2 textureS = vec2(16, 17);
	float y = floor(float(INDEX)/textureS.y);
	float x = floor(float(INDEX) - textureS.y*y);
	vec2 pixelCoord = vec2(x, y);
	COLOR = texture(mainTexture, (pixelCoord+vec2(0.5)) / vec2(textureSize(mainTexture, 0)));
	COLOR = getOutlines((pixelCoord+vec2(0.5)) / vec2(textureSize(mainTexture, 0)));
	TRANSFORM[3].xy = pixelCoord - textureS/2.;
	TRANSFORM[3].xy = RotateVec(TRANSFORM[3].xy, rotation);
	VELOCITY.xy = vec2(0.);
}

void process() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	if(RESTART){
		float randRadialVelValue = (rand_from_seed(alt_seed) - 0.5)*0.1;
		VELOCITY.xy = RotateVec(normalize(TRANSFORM[3].xy), PI/2.) * 90. * (randRadialVelValue + normalize(randRadialVelValue)*0.5);
		VELOCITY.xy += normalize(TRANSFORM[3].xy) * 4.;
		if (length(initialDir) != 0.) VELOCITY.xy += normalize(initialDir)*20.;
		USERDATA1.w = 0.;
		USERDATA1.xy = vec2(0.);
	}
	VELOCITY.xy -= normalize(TRANSFORM[3].xy) * DELTA * 120. * (1. - length(TRANSFORM[3].xy/45.));
	VELOCITY.xy -= normalize(VELOCITY.xy) * DELTA * 15.;
	
	if (USERDATA1.w < 0.){
		USERDATA1.w = 10.;
		//COLOR.rgb = vec3(1., 0., 0.);
		USERDATA1.xy = TRANSFORM[3].xy;
	}

	if (USERDATA1.w == 10. && COLOR.a > 0.){// && dot(normalize(USERDATA1.xy), normalize(TRANSFORM[3].xy)) < 0.){
		float SoredPositionLength = length(USERDATA1.xy);
		float alpha1 = 1. - ((SoredPositionLength - length(TRANSFORM[3].xy))/SoredPositionLength);
		float alpha2 = dot(normalize(USERDATA1.xy), normalize(TRANSFORM[3].xy));
		COLOR.a = min(alpha1, alpha2);
	}
	
	if (USERDATA1.w != 10.) USERDATA1.w = dot(normalize(VELOCITY.xy), normalize(TRANSFORM[3].xy));
	
}