shader_type canvas_item;

uniform sampler2D sheet : filter_nearest;
uniform int tiles[4096];

bool hasAdjacentTile(ivec2 UVtile){
	for(int x = -1; x <= 1; x++)
		for(int y = -1; y <= 1; y++){
			if ((x == 0 && y == 0) || UVtile.x + x < 0 || UVtile.x + x > 63 || UVtile.y + y < 0 || UVtile.y + y > 63)
				continue;
			if (tiles[(UVtile.x + x) + (UVtile.y + y)*64] != -1)
				return true;
		}
		return false;
	}
		
bool shouldDrawAdjacentTile(ivec2 UVtile, ivec2 dir){
	if ((dir.x == 0 && dir.y == 0) || UVtile.x + dir.x < 0 || UVtile.x + dir.x > 63 || UVtile.y + dir.y < 0 || UVtile.y + dir.y > 63) 
		return false;
	if (tiles[(UVtile.x + dir.x) + (UVtile.y + dir.y)*64] != -1)
			return true;
	return false;
}

void fragment() {
	int sheetTilesSize = 20;
	ivec2 UVtile = ivec2(UV*64.);
	int tileIdx = tiles[UVtile.x + UVtile.y*64];
	ivec2 tileInSheet = ivec2(tileIdx % sheetTilesSize, tileIdx / sheetTilesSize);
	vec2 tileLocalUV = fract(UV * 64.);
	//COLOR.a = 0.;
	if (tileIdx == -1){
		COLOR = vec4(0);
		vec2 absCTileLocalUV = abs((tileLocalUV-vec2(0.5))*2.);
//		ivec2 borderDir = ivec2((ceil(tileLocalUV - .5)*2.)-1.);
		if ((absCTileLocalUV.x > 12./16. || absCTileLocalUV.y > 12./16.)
		 && shouldDrawAdjacentTile(UVtile, ivec2((ceil(tileLocalUV - .5)*2.)-1.))){
			vec2 dir = (tileLocalUV * 2.) -1.;
			
			dir = ((ceil(dir)*2.)-1.) * (ceil(abs(dir*(8.000000001/6.)))-1.);
			
			tileIdx = tiles[UVtile.x + int(dir.x) + ((UVtile.y + int(dir.y))*64)];
			tileInSheet = ivec2(tileIdx%sheetTilesSize, tileIdx/sheetTilesSize);
			vec2 texuv = (tileLocalUV/(400./16.) + vec2(tileInSheet)/20. + vec2(2.)/400. + (vec2(-16)*dir)/400.);
			COLOR = texture(sheet, texuv);
			//COLOR = vec4(1., abs(dir.x), abs(dir.y), 1.);
		}
	}
	else COLOR = texture(sheet, tileLocalUV/(400./16.) + vec2(tileInSheet)/20. + vec2(2./400.));
	//COLOR.a *= 0.85;
}
