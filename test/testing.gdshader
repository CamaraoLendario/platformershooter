shader_type canvas_item;

void vertex() {
	VERTEX *= 2.;
}

void fragment() {
	COLOR = texture(TEXTURE, UV + vec2(0.5, 0));
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
