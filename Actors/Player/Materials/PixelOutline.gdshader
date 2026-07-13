shader_type canvas_item;
uniform vec3 Color : source_color = vec3(0.8, 0.8, 0.0);
const vec2[4] directions = vec2[4](
	vec2(1., 0.),
	vec2(0., 1.),
	vec2(-1., 0.),
	vec2(0., -1.)
);

float HasAjacentPixels(sampler2D tex, vec2 uv, vec2 pixelSize){
	if (texture(tex, uv).a > 0.) {
		return 0.;
	}
	for(int i = 0; i < 4; i++){
		if (texture(tex, uv + (directions[i]*pixelSize)).a > 0.){
			return 1.;
		}
	}
	return 0.;
}

void fragment() {
	COLOR = mix(COLOR, vec4(Color, 1.), HasAjacentPixels(TEXTURE, UV, TEXTURE_PIXEL_SIZE));
}