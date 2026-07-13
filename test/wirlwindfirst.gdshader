shader_type canvas_item;
#include "res://test/usefulFuncs.gdshaderinc"

uniform vec3[4] Colors : source_color;
uniform vec2[12] waveSum;

float GetSinWaveSum(float angle, int waveSumIdx){
	float value = 0.; 
	for(int i = 0; i < 4; i++) {
		value += (sin((angle * waveSum[i + (waveSumIdx * 4)].x) + ((TIME*0.1) * waveSum[i + (waveSumIdx * 4)].y)) + 1.) / 4.;
	}
	return value;
}

float GetCosWaveSum(float angle, int waveSumIdx){
	float value = 0.; 
	for(int i = 0; i < 4; i++) {
		value += (cos((angle * waveSum[i + (waveSumIdx * 4)].x) + ((TIME*0.1) * waveSum[i + (waveSumIdx * 4)].y)) + 1.) / 4.;
	}
	return value;
}


void fragment() {
	vec2 uv = UV * 2. - 1.;
	
	float value = length(uv);
	
	//uv = RotateVec(uv, value*2.);
	
	float angle = atan(uv.y, -uv.x) + PI;
	
	float[4] Values;
	
	Values[0] = ((GetSinWaveSum(angle, 0)*0.7)+0.3);
	Values[1] = ((GetSinWaveSum(angle, 1)*0.6)+0.4);
	Values[2] = ((GetSinWaveSum(angle, 2)*0.5)+0.5);
	
	Values[3] = 10.;
	
	value *= 3.;
	vec3 Color;
	float heightOffset;
	float lighting;
	for(int i = 0; i < 4; i++){
		if (i < 3){
			int index = i;
			//if (fract(value) < 0.5){
				//index = max(1, i-1);
			//}
			
			if (value > float(index+1) + Values[index]) continue;
			heightOffset = (1. - (value - float(i)) + Values[i]);
			lighting = GetCosWaveSum(angle, index);
			Color = Colors[i].rgb;
			break;
		}
		else {
			Color = Colors[3].rgb;
			heightOffset = 1.;
		}
	}
	float test = (1. - (value - float(1)) + Values[1]);
	//heightOffset = 1.-abs(heightOffset*2. -1.);
	//COLOR.rgb = Color * (lighting + 0.3) + heightOffset*heightOffset*0.4;
	COLOR.rgb = Color * (min(1., heightOffset));
}
