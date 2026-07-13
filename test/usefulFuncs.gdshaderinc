uint hash(uint x) {
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = (x >> uint(16)) ^ x;
	return x;
}
vec4 getOutlines(vec2 texUV, vec4 currentColor, sampler2D mainTexture, vec3 outlineColor){
	if (currentColor.a > 0.) return currentColor;
	
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

vec2 cossin(float angle){
	return vec2(cos(angle), sin(angle));
}
