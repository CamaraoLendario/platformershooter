shader_type canvas_item;

uniform vec4 Color: source_color = vec4(0.9, 0.0, 0.0, 1.0);
const float value = 2.;

void fragment() {
	float a = COLOR.a;
	COLOR = (COLOR + (Color*value))/(1.+value);
	COLOR.a = a;
}
