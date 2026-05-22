shader_type canvas_item;
uniform vec3 teamColor: source_color = vec3(1.);
uniform float Scale = 4.;
uniform float pointsValue = 0;
uniform float separation = 0.6;
uniform float offsetStrength = 1;
uniform float RandHOffset = 10;
uniform float animationSpeed = 1;

uint hash(uint x) {
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = (x >> uint(16)) ^ x;
	return x;
}
float GetRand(float offset){
	return fract(float(hash(uint(offset)))/100000.);
}
vec2 GetRandVec(float offset){
	return vec2(
		(GetRand(offset + pointsValue * 18957.34521)-.5),
		(GetRand(offset + pointsValue * 99213.21607)-.5)
	)*1.5;
}

void vertex() {
	VERTEX.y *= Scale;
	
	VERTEX.x += sin(TIME * animationSpeed * 4.)*2.*(1.-UV.y);
	VERTEX.y -= cos((TIME * animationSpeed * 8.))*2.*(1.-UV.y);
	VERTEX *= 3.;
}

void fragment() {
	
	vec2 uv = UV * vec2(1., Scale);
	uv.y -= Scale - (pointsValue*Scale);
	
	COLOR *= 0.;
	vec2 newUV;
	for (float i = 0.; i < Scale*2.; i++){
		newUV = uv;
		newUV.x = abs(float(int(i) % 2) - uv.x);
		newUV = newUV - vec2(
			(GetRand(i * 18957.34521)/10.)*RandHOffset,
			separation*i);
		newUV += (GetRandVec(i)*offsetStrength) * TEXTURE_PIXEL_SIZE*1.;
		vec4 newColor = texture(TEXTURE, newUV);
		if (newColor.a != 0.){
			int value = int(i)%2;
			value = (value * 2)-1;
			COLOR = newColor;
			COLOR.rgb *= (i+7.)/((Scale*2.)+7.);
		}
	}
	
	COLOR.rgb *= 1.5;
	COLOR.rgb *= teamColor;
	
	//float value = (UV.y+1.)/2.;
	//COLOR.rgb -= (COLOR.rgb/5.) * value;
}
