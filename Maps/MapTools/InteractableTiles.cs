using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class InteractableTiles : TileMapLayer
{
	Map mapNode;
	PackedScene destructibleBlockFlagScene = GD.Load<PackedScene>("uid://cyhc0waevvoee");
	TileMapLayer destructibleBlockFlags;
	List<(Vector2I coord, int sourceId, Vector2I atlasCoords)> tileMapData =[];
	Vector2I checkArea;
	public override void _Ready()
	{
		mapNode = GetParent<Map>();
		//destructibleBlockFlags = GetNode<TileMapLayer>("DestructibleTiles");  <- is this necessary?
		checkArea = mapNode.MapBordersNode.MapSize + new Vector2I(2, 2);

		tileMapData = GetTileMapData();
		ResetDestructibleBlocksChecks();
	}

	
	List<(Vector2I coord, int sourceId, Vector2I atlasCoords)> GetTileMapData()
    {
        List<(Vector2I coord, int sourceId, Vector2I atlasCoords)> currentTileMapData = [];

		for (int Y = -checkArea.Y/2; Y <= checkArea.Y/2; Y++)
		{
			for (int X = -checkArea.X/2; X <= checkArea.X/2; X++)
			{
				Vector2I currentCoords = new Vector2I(X, Y);
				int currentSourceId = GetCellSourceId(currentCoords);
				Vector2I atlasCoords = GetCellAtlasCoords(currentCoords);
				
				currentTileMapData.Add((currentCoords, currentSourceId, atlasCoords));
			}
		}
		

		return currentTileMapData;
    }

	void ResetDestructibleBlocksChecks()
	{
		destructibleBlockFlags = GetNode<TileMapLayer>("DestructibleTiles");
		foreach (DestructibleBlockFlag oldFlag in destructibleBlockFlags.GetChildren())
		{
			oldFlag.QueueFree();
		}

		for (int Y = -checkArea.Y/2; Y <= checkArea.Y/2; Y++)
		{
			for (int X = -checkArea.X/2; X <= checkArea.X/2; X++)
			{
				Vector2I currentCoord = new Vector2I(X, Y);
				TileData currentTileData = GetCellTileData(currentCoord);

				if (currentTileData == null) continue;

				if ((bool) currentTileData.GetCustomData("Destructible"))
				{
					destructibleBlockFlags.SetCell(currentCoord, GetCellSourceId(currentCoord), GetCellAtlasCoords(currentCoord));
					SetCell(currentCoord, -1);
					DestructibleBlockFlag destructibleBlockFlag = destructibleBlockFlagScene.Instantiate<DestructibleBlockFlag>();
					destructibleBlockFlag.Position = currentCoord * 16;
					destructibleBlockFlag.tileMapLayer = destructibleBlockFlags;
					destructibleBlockFlags.AddChild(destructibleBlockFlag);
				}
			}
		}
	}

	public void Reset()
    {
        foreach((Vector2I coord, int sourceId, Vector2I atlasCoords) in tileMapData)
        {
            SetCell(coord, sourceId, atlasCoords);
        }

		ResetDestructibleBlocksChecks();
    }
}
