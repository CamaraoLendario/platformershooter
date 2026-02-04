shader_type canvas_item;
//Global var
global uniform vec2 cameraDistFromOrigin = vec2(0.);

//Set by user
uniform float sideLength = 2;
uniform vec2 scrollDir = vec2(1., 0);
uniform float scrollSpeed = 0.0;
uniform float parallaxMultiplier = 1;
uniform bool repeats = true;

//Set by parentNode
uniform vec4 textureRegion = vec4(0, 0, 640, 360);
uniform vec2 texSize = vec2(640, 2880);
uniform vec2 separation = vec2 (300, 300);

void fragment() {
	vec2 recRatio = texSize / textureRegion.zw;
	vec2 scrollLength = normalize(vec2(scrollDir)) * TIME * scrollSpeed * 0.1;
	vec2 uv;
	if (repeats)
		uv = (fract(UV * recRatio * sideLength + scrollLength + ((cameraDistFromOrigin * parallaxMultiplier) * TEXTURE_PIXEL_SIZE * recRatio)) / recRatio) + textureRegion.xy * TEXTURE_PIXEL_SIZE;
	else uv = (fract(UV * recRatio * sideLength + scrollLength) / recRatio) + textureRegion.xy * TEXTURE_PIXEL_SIZE;
	COLOR = texture(TEXTURE, uv);
}
