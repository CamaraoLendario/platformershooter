shader_type canvas_item;

const vec2[2] dirs = vec2[2](
	vec2(1, 0),
	vec2(0, 1)
);

void fragment() {
	vec2 uv = (UV*2.) - vec2(1.);
	//COLOR.rg = UV;
	
	COLOR *= 1. - smoothstep(1, 1, length(uv));
	COLOR.rgb = vec3(0.);
	
	vec2 inputVec = (UV-vec2(0.5))*2.;
	inputVec *= 1.9;
	if (length(inputVec) > 1.){
		COLOR.rgb = vec3(vec2(0.5), 0.75);
		
		float angle = asin(inputVec.y);
		//COLOR.r = (angle/PI); 
		if (inputVec.x > 0.55)
			COLOR.rgb += vec3(0.5, 0., 0.);
		if (inputVec.y > 0.55)
			COLOR.rgb += vec3(0., 0.5, 0.);
		if (inputVec.x < -0.55)
			COLOR.rgb += vec3(-0.5, 0., 0.);
		if (inputVec.y < -0.55)
			COLOR.rgb += vec3(0., -0.5, 0.);
	}
}
