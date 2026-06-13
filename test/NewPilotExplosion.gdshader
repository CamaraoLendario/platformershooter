shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"

uniform vec2 initialDir = vec2(1.);
uniform sampler2D mainTexture;
uniform vec3 outlineColor: source_color = vec3(1., 0., 0.);


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
	COLOR = getOutlines(texUV, COLOR, mainTexture, outlineColor);
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
