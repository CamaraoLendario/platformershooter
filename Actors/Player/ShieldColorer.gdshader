shader_type canvas_item;

uniform vec4 Color: source_color = vec4(0.9, 0.0, 0.0, 1.0);

void fragment() {
	float a = COLOR.a;
	COLOR = (COLOR + (Color*2.))/(3.);
	COLOR.a = a;
}
