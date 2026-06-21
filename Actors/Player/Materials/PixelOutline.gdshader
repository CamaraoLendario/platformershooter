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

//vec4 GetOutline(sampler2D mainTexture, vec2 texUV, vec4 color){
	//if (color.a != 0.) return color;
	//
	//vec2 quadrant = (ceil((texUV*2.)-vec2(1.))*2.)-vec2(1.);
	//vec2 Hquadrant = quadrant * vec2(1, 0);
	//vec2 Vquadrant = quadrant * vec2(0, 1);
	//vec2 dirs[4] = {
		//-Hquadrant,
		//-Vquadrant,
		//Hquadrant,
		//Vquadrant
		//};
	//for (int i = 0; i < 4; i++){
		//vec2 HUVoffset = dirs[i] / vec2(textureSize(mainTexture, 0));
		//float alpha = texture(mainTexture, texUV + HUVoffset).a;
		//if (alpha != 0.) return vec4(Color, 1.);
	//}
	//return vec4(0.);
//}

void fragment() {
	if (COLOR.a == 0. && GetAjacentCount(TEXTURE, UV, TEXTURE_PIXEL_SIZE) > 0.0){
		COLOR = vec4(Color, 1.0);
	}
	//COLOR.a += 0.1;
	//COLOR = GetOutline(TEXTURE, UV, COLOR);
}
