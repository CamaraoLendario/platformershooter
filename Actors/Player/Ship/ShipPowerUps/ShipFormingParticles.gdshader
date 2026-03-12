shader_type particles;

uniform sampler2D mainTexture;
uniform vec3 outlineColor: source_color = vec3(1., 0., 0.);
uniform vec2 initPos = vec2(0.);
uniform vec2 newPos = vec2(0.);
uniform float rotation;


vec4 getOutlines(vec2 pos){
	vec4 selfColor = texture(mainTexture, pos); 
	if (selfColor.a != 0.) return selfColor;
	
	for(int y = -1; y <= 1; y++){
		for(int x = -1; x <= 1; x++){
			if ((x + y)%2 == 0) continue;
			if (texture(mainTexture, pos + vec2(float(x), float(y))/vec2(32., 32.)).a != 0.){
				return vec4(outlineColor, 1.);
			}
		}
	}
	return vec4(0.);
}

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
	vec2 textureS = vec2(textureSize(mainTexture, 0));
	float y = floor(float(INDEX)/textureS.y);
	float x = floor(float(INDEX) - textureS.x*y);
	vec2 endPos = vec2(x, y) - vec2(15.5);
	vec2 pixelPos = (endPos / vec2(textureSize(mainTexture, 0)) + vec2(0.5, 0.5));
	USERDATA1.xy = endPos;
	COLOR = texture(mainTexture, pixelPos);
	//COLOR = getOutlines(pixelPos);
	TRANSFORM[3].z = 0.;
}

void process() {
	if(RESTART){
		uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
		float angle = rand_from_seed(alt_seed)*PI*2.;
		float dist = rand_from_seed(alt_seed);
		
		ivec2 size = textureSize(mainTexture, 0)/2;
		TRANSFORM[3].xy = -normalize(USERDATA1.xy - initPos) * 30. + vec2(cos(angle), sin(angle)) * dist * 30.;
		
		USERDATA1.zw = TRANSFORM[3].xy + initPos; //initialPos
		TRANSFORM[3].z = 0.;
	}
	uint test = hash(INDEX);
	float indexRandomness = rand_from_seed(test);
	vec2 endPos = RotateVec(USERDATA1.xy, rotation) + newPos;
	vec2 travelVec = endPos - USERDATA1.zw;
	TRANSFORM[3].z += DELTA;///LIFETIME;
	float value = TRANSFORM[3].z;
	value = min(value, 1.);
	if (COLOR.a != 0.) COLOR.a = smoothstep(0., 1., value);
	float directValue =  sin(value*PI/2.);
	float flipVecDir = 1.;
	if (int(indexRandomness*20.) % 2 == 0) flipVecDir = -1.;
	vec2 perpendicularVec = RotateVec(normalize(travelVec) * 50. * rand_from_seed(test), PI/2. * flipVecDir);
	float perpendicularValue = sin(value*PI) * 0.25;
	
	float currentRotation = rotation * value;
	float c = cos(-currentRotation);
	float s = sin(-currentRotation);
	TRANSFORM[0].xyz = vec3(c, -s, 0.0);
	TRANSFORM[1].xyz = vec3(s,  c, 0.0);
	
	TRANSFORM[3].xy = USERDATA1.zw + (travelVec * directValue) + (perpendicularVec * perpendicularValue);
}
