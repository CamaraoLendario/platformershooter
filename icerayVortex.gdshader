shader_type particles;

uniform float particlesPerVortex = 8.;
uniform float vortexHeight = 20.;
uniform vec3 color : source_color = vec3(1.);
uniform vec2 dir = vec2(1., 0.);
uniform float particles = 240.;

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
	vec2 offset = vec2(cos(angle), sin(angle));
	 //offset *= 1. - float(INDEX)/particles; // taper down vortex
	vec2 pos = offset * vortexHeight/2.;
	pos /= 2.;
	//pos.x *= 0.5;
	pos.xy += normalize(dir) * (float(INDEX) / 3.);
	TRANSFORM[3].xy = pos;
	COLOR.a = min(1., 1. - dot(normalize(offset), normalize(dir)));
	COLOR.a *= 1. - (USERDATA1.w/1.);
}
