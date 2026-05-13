shader_type canvas_item;
uniform vec2 texSize = vec2(16,16);
uniform float scale = 3.; 
uniform int imagesCount = 5;
uniform vec2[5] afterImagePos;

void vertex() {
	VERTEX -= texSize/2.;
	float normalizedScale = max(1., scale);
	VERTEX *= normalizedScale;
	VERTEX += texSize/2.;
}

void fragment() {
	vec2 uv = (UV - vec2(.5)) * 2.;
	
	float value = min(texSize.x, texSize.y);
	uv *= vec2(texSize.x/value, texSize.y/value);
	
	float normalizedScale = max(1., scale);
	uv *= normalizedScale;
	COLOR *= 0.;
	for(int i = 0; i < 5; i++){
		vec2 tUV = uv + afterImagePos[i] * normalizedScale/2.;
		if (abs(tUV.x) < 1. && abs(tUV.y) < 1.){
			vec4 newColor = texture(TEXTURE, ((tUV)+vec2(1))/2.);
			if (newColor.a != 0.){
				COLOR.rgb = newColor.rgb;
				COLOR.a = (float(i+1))/6.;
			}
		}
	}
	//if (abs(uv.x) < 1. && abs(uv.y) < 1.)
		//
		//COLOR = texture(TEXTURE, (uv+vec2(1))/2.);
	//else COLOR.rgba = vec4(UV, 0., 1.);
}

