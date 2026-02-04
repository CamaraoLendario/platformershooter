using Godot;
using System;
using System.Net;

public partial class DrillShot : LinearProjectile
{
    public override void End(TileMapLayer tileMapLayer)
    {
		if(tileMapLayer is InteractableTiles)
		{
			End();
			return;	
		}

		foreach(Area2D area in GetOverlappingAreas())
        {
			if (area is not DestructibleBlockFlag destructibleBlockFlag) continue;
			destructibleBlockFlag.Destroy();
        }
    }
}