shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;
void fragment() {
	vec3 screen_pixel = texture(SCREEN_TEXTURE, SCREEN_UV).rgb;
	screen_pixel.r = 1.0;
	COLOR = vec4(screen_pixel, 1);
}
