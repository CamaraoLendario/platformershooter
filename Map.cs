using Godot;
using System;
using System.Linq;
using System.Runtime.InteropServices;

[Tool]
public partial class MapWheelReinvented : Node2D
{
	Texture2D sheet;
	struct Tile {
		public int sheetPos;
		//Vector2I size; for now this will be assumed 16x16
		//rotation?
		//fliph???
		//flipv???
	}
	Tile emptyTile = new Tile
	{
		sheetPos = -1,
	};

	Tile[] tiles = new Tile[64*64];

	public override void _Ready()
	{
		TextureRect texrec = GetChild<TextureRect>(1);
		ShaderMaterial shader = texrec.Material as ShaderMaterial;
		
		int[] emptyTiles = new int[64*64];
		Array.Fill(emptyTiles, -1);

		shader.SetShaderParameter("tiles", emptyTiles);
	}

	void PlaceBlock(Vector2I pos, Tile tile)
	{
		tiles[pos.X + pos.Y*64] = tile;
	}

	void RemoveBlock(Vector2I pos)
	{
		tiles[pos.X + pos.Y*64] = emptyTile;
	}

	/* Texture2D GetMapTexture(int layer)
	{

	} */
}
