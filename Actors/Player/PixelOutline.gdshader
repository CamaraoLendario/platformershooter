shader_type canvas_item;
uniform float PoisonBuildup = 0.0;
uniform vec3 Color : source_color = vec3(0.8, 0.8, 0.0);

float GetAjacentCount(sampler2D tex, vec2 uv, vec2 pixelSize){
	float pixelCount = 0.0;
	if (texture(tex, uv).a > 0.0) return 0.0;
	for(int x = -1; x <= 1; x++){
		for(int y = -1; y <= 1; y++){
			if (texture(tex, uv + vec2(float(x), float(y)) * pixelSize).a > 0.0){
				if (x == 0 || y== 0) pixelCount += 1.0;
			}
		}
	}
	return pixelCount;
}

void fragment() {
	if (COLOR.a > 0.0){
		vec3 poisonColor = vec3(-0.2, 0.2 , -0.2);
		float greenIncrease = ((UV.y-1.0) + PoisonBuildup);
		if (greenIncrease > 0.0){
			COLOR.rgb += ((UV.y-1.0) + PoisonBuildup) * poisonColor;
			COLOR.rgb -= vec3(0.1, 0.0, 0.1);
		}
	}

	if (GetAjacentCount(TEXTURE, UV, TEXTURE_PIXEL_SIZE) > 0.0){
		COLOR = vec4(Color, 1.0);
	}
}
