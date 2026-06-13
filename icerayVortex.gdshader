shader_type particles;

uniform float particlesPerVortex = 8.;
uniform float vortexHeight = 20.;
uniform vec3 color : source_color = vec3(1.);
uniform vec2 dir = vec2(1., 0.);

void start() {
	USERDATA1.w = 0.;
}

void process() {
	USERDATA1.w += DELTA;
	COLOR.rgb = color;
	float angularSpeed = .5;
	float forward = TIME * angularSpeed * PI * 2.;
	float angle = (float(INDEX) / particlesPerVortex) * PI * 2.;
	angle -= forward;
	vec2 offset = vec2(cos(angle), sin(angle));// * max(1., 2. - (float(INDEX)/100.));
	//offset *= 1. - float(INDEX)/particles; // taper down vortex
	
	vec2 pos = offset * vortexHeight/2.;
	pos /= 2.;
	//pos.x *= 0.5;
	pos += normalize(dir) * (float(INDEX) / 3.);
	vec2 velDir = pos - TRANSFORM[3].xy;
	float currentRotation = atan(velDir.y / velDir.x);
	TRANSFORM[3].xy = pos;
	
	COLOR.a = min(1., 1. - dot(normalize(offset), normalize(dir)));
	COLOR.a *= 1. - (USERDATA1.w/1.);
	//COLOR.a = 1.;
	float c = cos(-currentRotation);
	float s = sin(-currentRotation);
	TRANSFORM[0].xyz = vec3(c, -s, 0.0);
	TRANSFORM[1].xyz = vec3(s,  c, 0.0);
}
