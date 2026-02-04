shader_type canvas_item;

uniform vec2 borderSize = vec2(832.0, 480.0);
uniform vec2 rectSize = vec2 (896.0, 544.0);
uniform vec2 position = vec2(-853, -42);

uniform float showRadius = 32;

float GetLength(vec2 vec){
	return sqrt((vec.x * vec.x) + (vec.y * vec.y));
}

void fragment(){
	COLOR = vec4(1, 1, 1, 0);
	vec2 pixelCoord = UV*rectSize + position + rectSize / 2.0;
	if(GetLength(UV - 0.5) > 0.5){
		discard;
	}
	
	float top = (rectSize - borderSize).y/2.0;
	float down = ((rectSize - borderSize).y/2.0) + borderSize.y;
	float left = (rectSize - borderSize).x/2.0;
	float right = ((rectSize - borderSize).x/2.0) + borderSize.x;
	
	
	if(
		pixelCoord.x < left && pixelCoord.x > left - 2.0 && pixelCoord.y < down + 2.0 && pixelCoord.y > top - 2.0 ||
		pixelCoord.x > right && pixelCoord.x < right + 2.0 && pixelCoord.y < down + 2.0 && pixelCoord.y > top - 2.0 ||
		pixelCoord.y < top && pixelCoord.y > top - 2.0 && pixelCoord.x > left - 2.0 && pixelCoord.x < right + 2.0 ||
		pixelCoord.y > down && pixelCoord.y < down + 2.0 && pixelCoord.x > left - 2.0 && pixelCoord.x < right + 2.0
	){
		COLOR = vec4(1, 1, 0.5, 1);
	}
	
}