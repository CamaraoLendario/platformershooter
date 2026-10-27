shader_type canvas_item;
//render_mode blend_premul_alpha;

uniform sampler2D sheet : filter_nearest;
uniform int tiles[4096];

int GetType(int tileIdx){
	if (tileIdx < 0 || tileIdx > 4095) return 9999;
	return ((tileIdx/80) * 5) + (tileIdx/4)%5;
}

void fragment() {
	int sheetTilesSize = 20;
	ivec2 UVtile = ivec2(UV*64.);
	int tileIdx = tiles[UVtile.x + UVtile.y*64];
	ivec2 tileInSheet = ivec2(tileIdx % sheetTilesSize, tileIdx / sheetTilesSize);
	vec2 tileLocalUV = fract(UV * 64.);
	
	vec2 dir = (tileLocalUV * 2.) -1.;
	dir = ((ceil(dir)*2.)-1.) * (ceil(abs(dir*(8.000000001/6.)))-1.);
	COLOR = vec4(0.);
	
	if (dir == vec2(0)){
		if (tileIdx < 0) 
			discard;
		COLOR = texture(sheet, tileLocalUV/(400./16.) + vec2(tileInSheet)/20. + vec2(2./400.));
	}
	else{
		
		int tilesIdxX = UVtile.x + int(dir.x) + UVtile.y*64;
		int tilesIdxY = UVtile.x + ((UVtile.y + int(dir.y))*64);
		int tileIdxs[3];
		tileIdxs[0] = tileIdx;
		if (tilesIdxX < 0 || tilesIdxX > 4095)
			tileIdxs[1] = -1;
		else tileIdxs[1] = tiles[tilesIdxX];
		
		if (tilesIdxY < 0 || tilesIdxY > 4095)
			tileIdxs[2] = -1;
		else tileIdxs[2] = tiles[tilesIdxY];
		if (max(max(tileIdxs[0], tileIdxs[1]), tileIdxs[2]) < 0) discard;
		int typeIdx[3];
		typeIdx[0] = GetType(tileIdxs[0]);
		typeIdx[1] = GetType(tileIdxs[1]);
		typeIdx[2] = GetType(tileIdxs[2]);
		
		vec4 color[3];
		color[0] = texture(sheet, tileLocalUV/(400./16.) + vec2(tileInSheet)/20. + vec2(2./400.));
		tileInSheet = ivec2(tileIdxs[1] % sheetTilesSize, tileIdxs[1] / sheetTilesSize);
		color[1] = texture(sheet, tileLocalUV/(400./16.) + vec2(tileInSheet)/20. + vec2(2./400.) + (vec2(-16)*dir)/400.);
		tileInSheet = ivec2(tileIdxs[2] % sheetTilesSize, tileIdxs[2] / sheetTilesSize);
		color[2] = texture(sheet, tileLocalUV/(400./16.) + vec2(tileInSheet)/20. + vec2(2./400.) + (vec2(-16)*dir)/400.);
		
		if (typeIdx[0] > typeIdx[1]){
			int oldIdx = tileIdxs[0];
			int oldType = typeIdx[0];
			vec4 oldColor = color[0];
			
			tileIdxs[0] = tileIdxs[1];
			typeIdx[0] = typeIdx[1];
			color[0] = color[1];
			
			tileIdxs[1] = oldIdx;
			typeIdx[1] = oldType;
			color[1] = oldColor;
		}
		if (typeIdx[1] > typeIdx[2]){
			int oldIdx = tileIdxs[1];
			int oldType = typeIdx[1];
			vec4 oldColor = color[1];
			
			tileIdxs[1] = tileIdxs[2];
			typeIdx[1] = typeIdx[2];
			color[1] = color[2];
			
			tileIdxs[2] = oldIdx;
			typeIdx[2] = oldType;
			color[2] = oldColor;
		}
		if (typeIdx[0] > typeIdx[1]){
			int oldIdx = tileIdxs[0];
			int oldType = typeIdx[0];
			vec4 oldColor = color[0];
			
			tileIdxs[0] = tileIdxs[1];
			typeIdx[0] = typeIdx[1];
			color[0] = color[1];
			
			tileIdxs[1] = oldIdx;
			typeIdx[1] = oldType;
			color[1] = oldColor;
		}
		
		COLOR = vec4(0., 0., 0., 0.);
		for (int i = 0; i < 3; i++){
			if (typeIdx[i] == -1) continue;
			vec3 colorAdd = color[i].rgb * color[i].a;
			COLOR += vec4(colorAdd, color[i].a);
			if (COLOR.a >= 1.) break;
		}
	}
}
