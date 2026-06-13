shader_type canvas_item;

uniform float value = 0.;
uniform float cornerRadius = 0.05;
uniform vec2 rectSize = vec2(1., 1.);

vec2 GetQuadrant(vec2 uv){
	vec2 centeredUV = (uv - vec2(0.5))*2.;
	return (ceil(centeredUV)*2.)-1.;
}

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec2 quadrant = GetQuadrant(UV / vec2(value, 1.));
	vec2 trueQuadrant = GetQuadrant(UV);
	vec2 rectAspectRatio = vec2(
		rectSize.x/rectSize.y,
		1.
	);
	vec2 ratioUV = UV*rectAspectRatio;
	float Value = (value) - cornerRadius / rectAspectRatio.x;
	vec2 centeredUV = (UV - vec2(0.5)) *2.;
	float supposedSin = max(0, abs(centeredUV.y) - (1.-(cornerRadius*2.)))/(cornerRadius*2.);
	float circleXpushback = (sqrt(1.- (supposedSin*supposedSin))*cornerRadius)/rectAspectRatio.x;
	
	vec2 ceilQuadrant = ceil(quadrant*0.9);


	if (quadrant.x > 0.)
		COLOR.a = 1. - smoothstep(0., 0., UV.x - (Value + (circleXpushback)));
	else 
		COLOR.a = smoothstep(0., 0., (UV.x - (1.-circleXpushback)) + 1. - cornerRadius / rectAspectRatio.x);
	// TODO: MAKE THIS NOT USE BRANCHING STATEMENT CUZ IT WOULD BE COOL
	float backgroundValue;
	
	vec2 circlePos = (ceil(trueQuadrant*0.9) * rectAspectRatio) - (trueQuadrant * cornerRadius);
	
	if ((ratioUV-circlePos).x * trueQuadrant.x > 0. && (ratioUV-circlePos).y * trueQuadrant.y > 0.)
		backgroundValue = 1.- smoothstep(1., 1., distance(ratioUV, circlePos)/cornerRadius);
	else backgroundValue = 1.;
	COLOR.a += backgroundValue*.1;
}
