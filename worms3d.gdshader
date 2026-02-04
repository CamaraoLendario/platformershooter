shader_type spatial;
render_mode cull_disabled;

vec2 randomvec2vec2(vec2 input) {
    return fract(sin(vec2(dot(input,vec2(127.1,311.7)),dot(input,vec2(269.5,183.3))))*43758.5453);
}

float getHeight(vec2 uv){
	uv *= 10.;
	vec2 Iuv = floor(uv);
	vec2 fuv = fract(uv);
	int scanSideLength = 2;
	int wormLength = 8;
	float minDist = 1.;
	for(int Y = -scanSideLength; Y <= scanSideLength; Y++){
		for(int X = -scanSideLength; X <= scanSideLength; X++){
			vec2 xy = vec2 (float(X), float(Y));
			vec2 scanCell = Iuv + xy;
			vec2 cellPos = randomvec2vec2(scanCell);
			for(int Idx = 0; Idx < wormLength; Idx++){
				float time = float(Idx)*0.2 + TIME;
				vec2 newCellPos = sin((time + 6.23113 * cellPos));
				float dist = distance(fuv, newCellPos + xy);
				minDist = min(minDist, dist*3.);
			}
		}
	}
	return minDist;
}

void vertex() {
	float value = getHeight(UV);
	value = clamp(value, 0., 1.);
	value = sqrt(1.-(value*value));
	VERTEX.y += value*1.2;
}

void fragment(){
	vec3 color = vec3(.8, 0.5, .5)*0.8;
	float height = 1.-getHeight(UV);
	if (height < 0.1) discard;
	ALBEDO = vec3(height)*color*12.;
}