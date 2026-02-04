shader_type canvas_item;

vec2 centerUV(vec2 uv){
	return (uv-vec2(0.5)) * 2.;
}
vec2 resetUV (vec2 uv){
	return (uv*0.5) + 0.5;
}

vec2 offsetTile(vec2 uv, float amount){
	uv.x += -1. * step(1., mod(uv.y, 2.)) * clamp((fract(0.)-0.5)*2., 0.0, 1.);
	uv.y +=  step(1., mod(uv.x, 2.)) * clamp((fract(0.+.5)-0.5)*2., 0.0, 1.);
	//uv.y += step(1., mod(uv.x, 2.)) * amount;

	return fract(uv);
}

float box(vec2 uv, vec2 size){
	size = (vec2(1.) - size)/2.;
	vec2 value = smoothstep(size, size, uv);
	value *= smoothstep(size, size, vec2(1)-uv);

	return value.x * value.y;
}

float circle(vec2 uv, float radius, float AA){

	return smoothstep(radius, radius+AA, dot(uv, uv));
}

float triangleTile(vec2 uv, float AA){
	uv = resetUV (uv);
	float sum = fract(uv.x) + fract(uv.y);
	float value = smoothstep(1., 1. + AA, sum);
	return value;
}

float random(vec2 uv){
	float value = fract(sin(dot(uv, vec2(9.5879463,957.853467))) * 1537.7315);

	return value;
}

float random(float input){
	float output;
	output = sin(input * 133.523585678545) *  310.13415;

	return fract(output);
}

vec2 randomvec2vec2(vec2 input) {
    return fract(sin(vec2(dot(input,vec2(127.1,311.7)),dot(input,vec2(269.5,183.3))))*43758.5453);
}

float noise(vec2 uv){
	vec2 intuv = floor(uv);
	vec2 floatuv = fract(uv);

	float a = random(intuv);
	float b = random(intuv + vec2(1., 0.));
	float c = random(intuv + vec2(0., 1.));
	float d = random(intuv + vec2(1., 1.));

	vec2 smoothedfloatuv = smoothstep(0., 1., floatuv);

	float output =
	mix(a, b, smoothedfloatuv.x) +
	(c - a) * smoothedfloatuv.y * (1.0 - smoothedfloatuv.x) +
	(d - b) * smoothedfloatuv.x * smoothedfloatuv.y;


	return output;
}

float maze(vec2 uv, vec2 vec){

	float hash = random (vec);
	if (hash > .5){
		uv.y = 1. - uv.y;
	}

	float value = smoothstep(1.-0.1, 1.+0.1 , uv.x + uv.y);
	value *= smoothstep(1.+0.1, 1. - 0.1 , uv.x + uv.y);

	return value*3.;
}

vec2 skew(vec2 uv){
	vec2 skewed = vec2(0.);

	skewed.x = uv.x * 1.1547;
	skewed.y = uv.y+0.5*skewed.x;

	return fract(skewed);
}


vec3 simplexGrid(vec2 uv){
	vec3 rgb = vec3(0.);
	vec2 gridUV = skew(uv);
	if (gridUV.x > gridUV.y){
		rgb = vec3(gridUV.x, gridUV.y, 1.0);
	}
	else{
		rgb = vec3(gridUV.y, 1.-gridUV.x, 1.0);
	}
	return rgb;
}

vec2 getDistVec(vec2 uv){
	uv *= 1.;
	vec2 Iuv = floor(uv);
	vec2 fuv = fract(uv);
	int scanSideLength = 2;
	int wormLength = 8;
	float minDist = 10.;
	vec2 distVec;
	for(int Y = -scanSideLength; Y <= scanSideLength; Y++){
		for(int X = -scanSideLength; X <= scanSideLength; X++){
			vec2 xy = vec2 (float(X), float(Y));
			vec2 scanCell = Iuv + xy;
			vec2 cellPos = randomvec2vec2(scanCell);
			for(int Idx = 0; Idx < wormLength; Idx++){
				float time = float(Idx)*0.2 + TIME;
				vec2 newCellPos = sin((time + 6.23113 * cellPos));
				float dist = distance(fuv, newCellPos + xy);
				if (minDist > dist*3.){
					minDist = dist*3.;
					distVec =  newCellPos + xy - fuv;
				}
			}
		}
	}
	return distVec;
}

float getHeight(vec2 uv){
	uv *= 1.;
	vec2 Iuv = floor(uv);
	vec2 fuv = fract(uv);
	int scanSideLength = 2;
	int wormLength = 8;
	float minDist = 3.;
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
	minDist = smoothstep(0., 1., minDist);
	//minDist = sqrt(1.-(minDist*minDist));
	return 1.-minDist;
}
void fragment(){
	vec3 color = vec3(.8, 0.5, .5)*0.8;
	//vec2 distVec = getDistVec(UV*10.);
	//vec2 lightPos = normalize(vec2(cos(1), sin(1)));
	//COLOR.rgb = vec3(dot(distVec, lightPos))*color;
	float height = getHeight(UV*10.);
	if (height < 0.2){
		COLOR.a = 0.;
	}
	else{
		height *= 0.7;
		height += 0.3;
		
		COLOR.rgb = vec3(height)*color;
	}
}