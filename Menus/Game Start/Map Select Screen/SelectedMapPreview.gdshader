shader_type canvas_item;

uniform int Thickness = 0;
uniform vec3 color : source_color = vec3(1.0, 0, 0.5);

void fragment() {
	if (UV.x < float(Thickness) * TEXTURE_PIXEL_SIZE.x || UV.x > 1.0 - (float(Thickness) * TEXTURE_PIXEL_SIZE.x) ||
		UV.y < float(Thickness) * TEXTURE_PIXEL_SIZE.y || UV.y > 1.0 - (float(Thickness) * TEXTURE_PIXEL_SIZE.y))
		COLOR.rgb = color;
}