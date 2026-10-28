using Godot;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

[Tool]
public partial class MapTiles : Node2D
{
	[Export] public StaticBody2D staticBody;
	static readonly (int x, int y)[] ADJACENTS = [
		(1, 1),
		(0, 1),
		(-1, 1),
		(-2, 1),
		
		(1, 0),
		(0, 0),
		(-1, 0),
		(-2, 0),

		(1, -1),
		(0, -1),
		(-1, -1),
		(-2, -1),
		
		(1, -2),
		(0, -2),
		(-1, -2),
		(-2, -2),
	];

	Texture2D sheet;
	public struct Tile {
		public int tileType;
		public int tileOrientation;
	}
	Tile emptyTile = new Tile
	{
		tileType = -1,
	};

	Tile[] tiles = new Tile[64*64];
	public int[] tileIdxs = new int[64*64];
	Vector2I CurrentTilePos;
	ShaderMaterial shader;
	public override void _Ready()
	{
		ColorRect texrec = GetChild<ColorRect>(1);
		shader = texrec.Material as ShaderMaterial;
		
		tileIdxs = new int[64*64];
		Array.Fill(tiles, emptyTile);
		Array.Fill(tileIdxs, -1);

		updateShaderArray(tileIdxs);
	}
	void updateShaderArray(int[] newArray)
	{
		shader.SetShaderParameter("tiles", newArray);
	}

	public bool PlaceBlock(Vector2I pos, Tile tile)
	{
		if (tiles[pos.X + pos.Y*64].tileType == tile.tileType)
			return false;

		SetTile(pos, tile);
		
		checkAdjacents(pos);
		return true;
	}

	public bool RemoveBlock(Vector2I pos)
	{
		if (tiles[pos.X + pos.Y*64].tileType == emptyTile.tileType)
			return false;
		SetTile(pos, emptyTile);

		checkAdjacents(pos);
		return true;
	}

	void updateTileOrientation(Vector2I pos)
	{
		Tile tile = GetTile(pos);
		Vector2I[] checkDirs = [
			new Vector2I(-1, 0),
			new Vector2I(1, 0),
			new Vector2I(0, -1),
			new Vector2I(0, 1),
		];
		Vector2 adjacents = new Vector2(-2, -2);
		for(int i = 0; i < 4; i++)
		{
			if (GetTile(pos + checkDirs[i]).tileType != tile.tileType)
				continue;
			if (i < 2 && adjacents.X == -2)
				adjacents.X = 0;
			else if (i >= 2 && adjacents.Y == -2)
				adjacents.Y = 0;
			
			adjacents += checkDirs[i];
		}

		for (int i = 0; i < 16; i++)
		{
			if (ADJACENTS[i] == (adjacents.X, adjacents.Y))
			{
				tile.tileOrientation = i;
				break;
			}
		}
		
		SetTile(pos, tile);
	}

	Tile GetTile(Vector2I pos)
	{
		return GetTile(pos.X, pos.Y);	
	}
	Tile GetTile(int x, int y)
	{
		if (x < 0 || y < 0 || x > 63 || y > 63) return emptyTile;
		return tiles[x + y*64];
	}
	void SetTile(Vector2I pos, Tile tile)
	{
		SetTile(pos.X, pos.Y, tile);	
	}
	void SetTile(int x, int y, Tile tile)
	{
		if (x < 0 || y < 0 || x > 63 || y > 63) return;
		tiles[x + y*64] = tile;
		if (tile.tileType < 0)
			tileIdxs[x + y*64] = -1;
		else tileIdxs[x + y*64] = (tile.tileType%5 * 4) + (tile.tileType/5 * 80) + tile.tileOrientation % 4 + (tile.tileOrientation / 4)*20;
		updateShaderArray(tileIdxs);
	}

	void checkAdjacents(Vector2I pos)
	{
		Vector2I[] checkDirs = [
			new Vector2I(-1, 0),
			new Vector2I(1, 0),
			new Vector2I(0, -1),
			new Vector2I(0, 1),
		];
		foreach(Vector2I vec in checkDirs)
		{
			updateTileOrientation(new Vector2I(pos.X, pos.Y) + vec);
		}
		updateTileOrientation(new Vector2I(pos.X, pos.Y));
	}
}

