shader_type particles;
#include "res://test/usefulFuncs.gdshaderinc"
uniform sampler2D shipTexture;
uniform float initialDistanceFromCenter = 20;
uniform float spawningTime = 0.1;
uniform float rotation = 0.;
uniform float randomSpawnWeight = 0.5;
uniform float emitterRotationOffset = 0.;
uniform vec2 globalPosition;
const int emittersCount = 3;

void start() {
	uint alt_seed = hash(NUMBER + uint(1.) + RANDOM_SEED);
	COLOR.a = 1.;
	TRANSFORM[3].xy = USERDATA1.xy;
	ivec2 texSize = textureSize(shipTexture, 0);
	vec2 finalPosition = vec2(ivec2(
		int(INDEX) % texSize.x,
		int(INDEX) / texSize.x
	))+  vec2(.5);
	COLOR = texture(shipTexture, ((finalPosition)/vec2(texSize)));
	//finalPos
	USERDATA1.zw = finalPosition - (vec2(texSize)/2.);
	vec2 finalPosWithRotation = RotateVec(USERDATA1.zw, rotation);

	
	//USERDATA1.xy = vec2(cos(entryAngle), sin(entryAngle));
	//time
	USERDATA2.x = 0.;
	float maxHeight = length(vec2(32.));
	float normalizedHeight = ((1.-length(finalPosWithRotation)) + (maxHeight/2.)) / maxHeight;
	//delay time
	USERDATA2.y = spawningTime * 
	(normalizedHeight * 1.-randomSpawnWeight) + 
	(rand_from_seed(alt_seed) * randomSpawnWeight);
	
	float EmittersCount = float(emittersCount);
	vec2[3] spawnPoints;
	for(int i = 0; i < emittersCount; i++){
		float angle = (float(i) / EmittersCount) * PI * 2.;
		angle += emitterRotationOffset;
		spawnPoints[i] = vec2(cos(angle), sin(angle));
	}
	int spawnPointIdx = emittersCount-1;
	float currentDist = length(spawnPoints[spawnPointIdx] - finalPosWithRotation);
	
	for(int i = 0; i < emittersCount-1; i++){
		float newDist = length(spawnPoints[i] - finalPosWithRotation);
		if (currentDist > newDist){
			spawnPointIdx = i;
			currentDist = newDist;
		}
	}
	
	//initialPos
	USERDATA1.xy =  spawnPoints[spawnPointIdx] * initialDistanceFromCenter;
	//initialAlpha
	USERDATA2.z = COLOR.a;
	
}
void process() {
if (USERDATA2.z > 0.){
	USERDATA2.x += DELTA;
	float time = smoothstep(0., 1., (USERDATA2.x - USERDATA2.y) / (1. - spawningTime));
	float alpha = 0. * PI * 2.;
	alpha = time;
	 
	vec2 finalPos = RotateVec(USERDATA1.zw, rotation);
	vec2 directVector = finalPos - USERDATA1.xy;
	vec2 oldPos = TRANSFORM[3].xy;
	TRANSFORM[3].xy = (USERDATA1.xy + directVector * time) + globalPosition;
	
	vec2 normalizedVel = normalize(TRANSFORM[3].xy - oldPos);
	float currentRotation = mix(atan(max(0.001, normalizedVel.y)/max(0.001, normalizedVel.x)), rotation, time);
	float c = cos(-currentRotation);
	float s = sin(-currentRotation);
	TRANSFORM[0].xyz = vec3(c, -s, 0.0);
	TRANSFORM[1].xyz = vec3(s,  c, 0.0);
	
	COLOR.a = alpha/1.;
}}
