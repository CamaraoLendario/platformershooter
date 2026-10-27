shader_type canvas_item;

uniform sampler2D sheet : filter_nearest;
uniform int tiles[4096];
const int sheetTilesSize = 20;

ivec2 GetTileInSheet(int tileIdx){
	return ivec2(tileIdx % sheetTilesSize, tileIdx / sheetTilesSize);
}
int GetTileIdx(ivec2 UVtile, ivec2 dir){
	UVtile += dir;
	int idx = UVtile.x + UVtile.y*64;
	if (idx < 0 || idx > 4095)
		return -1;
	return tiles[idx];
}
int GetTypeIdx(int tileIdx){
	if (tileIdx < 0 || tileIdx > 4095) return 9999;
	return ((tileIdx/80) * 5) + (tileIdx/4)%5;
}
vec4 GetTileClr(vec2 localUV, int tileIdx){
	if (tileIdx == -1) return vec4(0);
	return texture(sheet, localUV/(400./16.) + vec2(GetTileInSheet(tileIdx))/20. + vec2(2./400.));
}
vec4 GetTileClrBorder(vec2 localUV, int tileIdx, vec2 dir){
	if (tileIdx == -1) return vec4(0);
	return texture(sheet, localUV/(400./16.) + vec2(GetTileInSheet(tileIdx))/20. + vec2(2./400.) + (vec2(-16)*dir)/400.);
}
void fragment() {
	ivec2 UVtile = ivec2(UV*64.);
	int tileIdx = GetTileIdx(UVtile, ivec2(0));
	int tileType = GetTypeIdx(tileIdx);
	ivec2 tileInSheet = GetTileInSheet(tileIdx);
	vec2 localUV = fract(UV * 64.);
	
	vec2 dir = (localUV * 2.) -1.;
	dir = ((ceil(dir)*2.)-1.) * (ceil(abs(dir*(8.000000001/6.)))-1.);
	COLOR = vec4(0.);
	
	COLOR = GetTileClr(localUV, tileIdx);
	if (dir == vec2(0)){
	}
	else if (abs(dir) != vec2(1)){
		int newIdx = GetTileIdx(UVtile, ivec2(dir));
		if (GetTypeIdx(newIdx) < tileType){
			vec4 brdrClr = GetTileClrBorder(localUV, newIdx, dir);
			if (brdrClr.a > 0.)
				COLOR = brdrClr;
		}
		else COLOR = GetTileClr(localUV, tileIdx);
	}
	else{
		vec2 dirs[3];
		dirs[0] = vec2(dir.x, 0);
		dirs[1] = vec2(0, dir.y);
		dirs[2] = dir;
		
		int Idxs[3];
		Idxs[0] = GetTileIdx(UVtile, ivec2(dirs[0]));
		Idxs[1] = GetTileIdx(UVtile, ivec2(dirs[1]));
		Idxs[2] = GetTileIdx(UVtile, ivec2(dirs[2]));
		
		int types[3];
		types[0] = GetTypeIdx(Idxs[0]);
		types[1] = GetTypeIdx(Idxs[1]);
		types[2] = GetTypeIdx(Idxs[2]);
		
		int j = 0;
		
		for (int i = 1; i < 3; i++){
			if (types[i] < types[j])
				j = i;
		}
		if (tileType > GetTypeIdx(Idxs[j])){
		vec4 newclr = GetTileClrBorder(localUV, Idxs[j], dirs[j]);
		if (newclr.a != 0.)
			COLOR = newclr;
		}
	}
	if (COLOR.a == 0.)
		COLOR.rgba = vec4(vec3(float((int(UV.x*64.) + int(UV.y*64.))%2)), 0.5);
}
