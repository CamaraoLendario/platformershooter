shader_type canvas_item;
uniform vec3 Color : source_color = vec3(0.8, 0.8, 0.0);
uniform int outlineThickness = 1;

vec2 GetCenteredUV(vec2 uv){
	return (uv - vec2(.5))*2.;
}
vec2 GetUncenteredUV(vec2 uv){
	return (uv + vec2(1))/2.;
}

void vertex(){
	VERTEX *= 2.;
	VERTEX -= 40.;
}

float GetAjacentCount(sampler2D tex, vec2 uv, vec2 pixelSize){
	float pixelCount = 0.0;
	//if (texture(tex, uv).a > 0.0) return 0.0;
	for(int x = -outlineThickness; x <= outlineThickness; x++){
		for(int y = -outlineThickness; y <= outlineThickness; y++){
			vec2 newUV = uv + vec2(float(x), float(y)) * pixelSize;
			if (newUV.x < 0. || newUV.x > 1. || newUV.y < 0. || newUV.y > 1.)
				continue;
			if (texture(tex, newUV).a > 0.0){
				if (x == 0 || y== 0)
					pixelCount += 1.0;
			}
		}
	}
	return pixelCount;
}

void fragment() {
	vec2 uv = GetUncenteredUV(GetCenteredUV(UV)*2.);
	if (uv.x > 0. && uv.x < 1. && uv.y > 0. && uv.y < 1.)
		COLOR = texture(TEXTURE, uv);
	else COLOR *= 0.;
	if (COLOR.a == 0. && GetAjacentCount(TEXTURE, uv, TEXTURE_PIXEL_SIZE) > 0.0){
		COLOR = vec4(Color, 1.0);
	}
	COLOR.a += 0.3;
}
