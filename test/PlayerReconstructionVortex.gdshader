shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"

uniform sampler2D mainTexture;
uniform vec3 outlineColor: source_color = vec3(1., 0., 0.);
uniform vec2 initialPos = vec2(0., -10.);
uniform float animationTime = 1.;
uniform float rotation = 0;
uniform bool flipped = false;

void start() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	ivec2 texSize = textureSize(mainTexture, 0);
	int idx = (texSize.x * texSize.y) - int(INDEX);
	
	// Position in texture (final position)
	USERDATA2.zw = vec2( 
		float(idx % texSize.x),
		float(idx / texSize.y)
	);
	
	//Centered UV
	vec2 uv = (USERDATA2.zw+vec2(0.5))/vec2(texSize);
	
	
	// Center the texture (final position)
	USERDATA2.zw = RotateVec(USERDATA2.zw - vec2(texSize/2), rotation);
	TRANSFORM[3].xy = (((vec2(rand_from_seed(alt_seed), rand_from_seed(alt_seed)) - vec2(0.5)) * 2.) * 1.5) + initialPos + (USERDATA2.w * vec2(0, 0.5));
	TRANSFORM[3].xy += vec2(-USERDATA3.x*2., 0.);
	//Initial Position
	USERDATA2.xy = TRANSFORM[3].xy;
	//TextureColor
	USERDATA1 = texture(mainTexture, uv);
	USERDATA1 = getOutlines(uv, USERDATA1, mainTexture, outlineColor);
	
	// Goes left(-1.) or right(1.)
	USERDATA3.x = (ceil(rand_from_seed(alt_seed)-0.5)*2.)-1.;
	// Rotation (it is not supposed to change during the animation)
	USERDATA3.y = rotation;
	// Initial delay
	USERDATA3.z = length(USERDATA2/16.);
	// Time since Emit Start
	USERDATA3.w = 0.;
}

void process() {
	USERDATA3.w += DELTA / (animationTime);
	float time = clamp(USERDATA3.w - USERDATA3.z, 0., 1.);
	float oldTime = clamp(USERDATA3.w - USERDATA3.z - DELTA, 0., 1.);
	COLOR = USERDATA1;
	float visibility = COLOR.a * smoothstep(0., 0., time);
	
	COLOR.a = max(time*5., 0.) * visibility;
	float timeprime = TIME + USERDATA3.z;
	vec2 normalVec = vec2(cos(timeprime*PI*2.), sin(timeprime*PI*2.))* 10.;
	vec2 OldPos = TRANSFORM[3].xy;
	TRANSFORM[3].xy = USERDATA2.zw - (normalVec*(1.-time));// * (1.-time) * 10.);
	
	vec2 normalizedVel;
	if (time < 1.){
		normalizedVel = normalize(TRANSFORM[3].xy - OldPos);
		
		float currentRotation = atan(normalizedVel.y/normalizedVel.x);// * value;
		currentRotation = mix(currentRotation, 0, time);
		float c = cos(-currentRotation-USERDATA3.y);
		float s = sin(-currentRotation-USERDATA3.y);
		TRANSFORM[0].xyz = vec3(c, -s, 0.0);
		TRANSFORM[1].xyz = vec3(s,  c, 0.0);
	}
	else{
		float c = cos(-USERDATA3.y);
		float s = sin(-USERDATA3.y);
		TRANSFORM[0].xyz = vec3(c, -s, 0.0);
		TRANSFORM[1].xyz = vec3(s,  c, 0.0);
	}
}
