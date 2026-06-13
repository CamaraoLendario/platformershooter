shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"

uniform sampler2D mainTexture;
uniform vec3 outlineColor: source_color = vec3(1., 0., 0.);
uniform vec2 initialPos = vec2(0., -10.);
uniform float rotation = 0;
uniform bool flipped = false;
uniform float spawningTime = 0.1;
uniform float spawningRandomizerWeight = 0.2;
uniform ivec2 sheetSize = ivec2(1);
uniform int frame = 0;

void start() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	ivec2 texSize = textureSize(mainTexture, 0) / sheetSize;
	int idx = int(INDEX);// (texSize.x * texSize.y) - int(INDEX);
	
	// Position in texture (final position)
	USERDATA2.zw = vec2( 
		float(idx % texSize.x),
		float(idx / texSize.x)
	)+vec2(.5);

	// Center the texture (final position)
	USERDATA2.zw -= vec2(texSize/2);
	//if (flipped){
		//USERDATA2.z *= -1.;
	//}
	TRANSFORM[3].xy = (((vec2(rand_from_seed(alt_seed), rand_from_seed(alt_seed)) - vec2(0.5)) * 2.) * 0.5) + initialPos + (USERDATA2.w * vec2(0, 0.5));
	TRANSFORM[3].xy += vec2(-USERDATA3.x*2., 0.);
	//Initial Position
	USERDATA2.xy = TRANSFORM[3].xy;
	
	// Goes left(-1.) or right(1.)
	USERDATA3.x = (ceil(rand_from_seed(alt_seed)-0.5)*2.)-1.;
	// Initial delay
	USERDATA3.z = spawningTime *
	clamp((rand_from_seed(alt_seed) * spawningRandomizerWeight) +
	((1.-(((RotateVec(USERDATA2.zw, rotation)).y+16.)/32.)) * (1.-spawningRandomizerWeight)), 0., 1.);
	// Time since Emit Start
	USERDATA3.w = 0.;
}

void process() {
	float flipFloat = flipped ? -1. : 1.;
	USERDATA3.w += DELTA/LIFETIME;
	float time = clamp(((USERDATA3.w * (1.+spawningTime)) - USERDATA3.z), 0., 1.);
	float oldTime = clamp((USERDATA3.w * (1.+spawningTime)) - (USERDATA3.z - (DELTA/LIFETIME)), 0., 1.);
	//TextureColor
	ivec2 texSize = textureSize(mainTexture, 0) / sheetSize;
	vec2 uv = (USERDATA2.zw +vec2(texSize/2)) / (vec2(texSize) *vec2(sheetSize));
	vec2 frameOffset = vec2(
		float(frame % sheetSize.x),
		float(frame / sheetSize.x)
	)/vec2(sheetSize);
	uv += frameOffset;
	USERDATA1 = texture(mainTexture, uv);
	USERDATA1 = getOutlines(uv, USERDATA1, mainTexture, outlineColor);
	COLOR = USERDATA1;
	float visibility = COLOR.a * smoothstep(0., 0., time);
	
	COLOR.a = max(time*5., 0.) * visibility;
	vec2 tranfrormVec = (RotateVec((USERDATA2.zw * vec2(flipFloat, 1.)), rotation) - USERDATA2.xy)*time;
	vec2 offsetVec = vec2(10.0 * USERDATA3.x, -10.)* sin(PI*time);
	tranfrormVec *= time;
	offsetVec *= sin(PI*time);
	vec2 OldPos = TRANSFORM[3].xy;
	TRANSFORM[3].xy = USERDATA2.xy + tranfrormVec + offsetVec;
	
	vec2 normalizedVel;
	if (time < 1.){
		normalizedVel = normalize(TRANSFORM[3].xy - OldPos);
		
		float currentRotation = atan(normalizedVel.y/normalizedVel.x);
		currentRotation = mix(currentRotation, 0, time);
		float c = cos(-currentRotation - rotation);
		float s = sin(-currentRotation - rotation);
		TRANSFORM[0].xyz = vec3(c, -s, 0.0);
		TRANSFORM[1].xyz = vec3(s,  c, 0.0);
	}
	else{
		float c = cos(-rotation);
		float s = sin(-rotation);
		TRANSFORM[0].xyz = vec3(c, -s, 0.0);
		TRANSFORM[1].xyz = vec3(s,  c, 0.0);
	}
	//COLOR = vec4(COLOR.xyz, 0.5);
}
