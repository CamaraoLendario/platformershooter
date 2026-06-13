shader_type canvas_item;
#include "res://test/usefulFuncs.gdshaderinc"

uniform sampler2D textureSheet: filter_nearest;
uniform vec2 texSize = vec2(18, 18);
uniform vec2 sheetSize = vec2(8, 1);
uniform float scale = 3.; 
uniform int imagesCount = 5;
uniform vec2[5] pixelsOffset;
uniform float[5] rotations;
uniform int[5] frames;
uniform bool[5] flip;

void vertex() {
	VERTEX -= texSize/2.;
	float normalizedScale = max(1., scale);
	VERTEX *= normalizedScale;
	VERTEX += texSize/2.;
}

void fragment() {
	//COLOR.a *= 0.;

	COLOR *= 0.;
	float normalizedScale = max(1., scale);
	//for (int i = 5; i >= 0; i--)
	for (int i = 0; i < imagesCount; i++)
	{
		vec2 framePos = vec2(ivec2(
			(frames[i] % int(sheetSize.x)),
			(frames[i] / int(sheetSize.x))
		));
		vec2 centeredUV = (UV - vec2(.5)) * 2.;
		
		vec2 rotatedUV = ((RotateVec(centeredUV, -rotations[i]))/2.) + vec2(.5);
		
		vec2 localUV = (rotatedUV * normalizedScale / sheetSize) + (framePos/sheetSize);
		
		localUV -= vec2((normalizedScale-1.)*.5)/sheetSize;
		localUV -= (RotateVec(-pixelsOffset[i]/texSize, -rotations[i]))/sheetSize;
		localUV = clamp(framePos/sheetSize, framePos/sheetSize + texSize/(texSize*sheetSize), localUV);
		if (flip[i])
		{
			localUV.x *= -1.;
			localUV.x += (framePos/sheetSize + texSize/(texSize*sheetSize)).x * 2.;
			localUV.x -= (framePos/sheetSize).x;
		}
		vec4 currentColor = texture(textureSheet, localUV);
		if (currentColor.a > 0.){
			currentColor.a = float(i+1)/6.;
			COLOR.rgb = currentColor.rgb;
			COLOR.a = currentColor.a;
		}
	}
	//COLOR.a += .1;
}
