shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture;
uniform float CAammount = 1.;
uniform float ConvexAmmount = 10.;
uniform bool enabled = true;

const vec2[2] dirs = vec2[2](
	vec2(1., 0.),
	//vec2(0., 1.),
	vec2(-1., 0.)//,
	//vec2(0., -1.)
);

void fragment() {
	if (enabled){
		
	
	vec2 uv = SCREEN_UV;
	uv = uv*2. - 1.;
	uv += normalize(uv) * length(uv) *  length(uv) * SCREEN_PIXEL_SIZE * ConvexAmmount;
	uv = (uv + 1.) / 2.;
	ivec2 iScreenSize = textureSize(screen_texture, 0);
//	float pixelDiff = SCREEN_PIXEL_SIZE.y / TEXTURE_PIXEL_SIZE.y;
	float pixelDiff = SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.y;
	vec2 screenSize = vec2(iScreenSize);
	
	
	COLOR.rgb = vec3(
		texture(screen_texture, uv - vec2(0., 1.) * SCREEN_PIXEL_SIZE * CAammount).r,
		texture(screen_texture, uv - vec2(-1., -1.) * SCREEN_PIXEL_SIZE * CAammount).g,
		texture(screen_texture, uv - vec2(1., -1.) * SCREEN_PIXEL_SIZE * CAammount).b
	);
	//for(int i = 0; i < 2; i++){
		//COLOR += texture(screen_texture, uv + (SCREEN_PIXEL_SIZE * dirs[i]));
	//}
	//COLOR.rgb /= 3.;
	if (
		uv.x > 1. ||
		uv.x < 0. ||
		uv.y > 1. ||
		uv.y < 0.){
		COLOR = vec4(0., 0., 0., 1.);
	}
	int value = int(UV.y * screenSize.y)%2;
	COLOR.rgb *= ((float(value)/2.)+.75);
	}
	else COLOR = texture(screen_texture, SCREEN_UV);
}