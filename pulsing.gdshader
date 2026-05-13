shader_type canvas_item;
uniform vec2 texSize = vec2(32);
uniform float baseScale = 1.2;

uint hash(uint x) {
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = (x >> uint(16)) ^ x;
	return x;
}

float GetPulse(){
	return sin(fract(TIME)*PI/2.);
}

vec2 centeruv(vec2 uv){
	return (uv - vec2(.5))*2.;
}
vec2 uncenteruv(vec2 uv){
	return (uv + vec2(1))/2.;
}
float getScale(){

	return baseScale - (fract(baseScale) * GetPulse());
}

float GetRand(float offset){
	return fract(float(hash(uint(offset * TIME)))/100000.);
}
vec2 GetRandVec(){
	return vec2(
		(GetRand(18957.34521)-.5)*2.,
		(GetRand(99213.21607)-.5)*2.
	);
}

void vertex() {
	VERTEX -= texSize/2.;
	float scale = getScale();
	VERTEX *= scale;
	VERTEX += texSize/2.;
}

void fragment() {
	vec2 uv = centeruv(UV);
	float scale = getScale();
	vec4 originalColor = texture(TEXTURE, (uncenteruv(uv*scale)));
	if (originalColor.a != 0.)
		COLOR = originalColor;
	else{
		vec2 randvec = GetRandVec()*0.03;
		COLOR = texture(TEXTURE, UV + (randvec * (GetPulse()-1.)));
		COLOR.a *= .5;
	}
}
