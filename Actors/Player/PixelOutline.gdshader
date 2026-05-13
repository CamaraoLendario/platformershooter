shader_type canvas_item;
uniform vec3 Color : source_color = vec3(0.8, 0.8, 0.0);
uniform int outlineThickness = 1;

float GetAjacentCount(sampler2D tex, vec2 uv, vec2 pixelSize){
	float pixelCount = 0.0;
	for(int x = -outlineThickness; x <= outlineThickness; x++){
		for(int y = -outlineThickness; y <= outlineThickness; y++){
			vec2 newUV = uv + vec2(float(x), float(y)) * pixelSize;
			if (texture(tex, newUV).a > 0.0){
				if (x == 0 || y== 0)
					pixelCount += 1.0;
			}
		}
	}
	return pixelCount;
}

void fragment() {
	if (COLOR.a == 0. && GetAjacentCount(TEXTURE, UV, TEXTURE_PIXEL_SIZE) > 0.0){
		COLOR = vec4(Color, 1.0);
	}
}
