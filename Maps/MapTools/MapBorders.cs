using Godot;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class MapBorders : Node2D
{
	//PackedScene collisionReplicatorScene = GD.Load<PackedScene>("uid://wli55setbe04");
	[Export]
	bool DrawBorders
	{
		get
		{
			return drawBorders;
		}
		set
		{
			drawBorders = value;
			UpdateDebug();
		}
	}
	private bool drawBorders;
	public enum StandardSizes {
		Size16by9,
		Size32by18,
		Size48by27,
		Size64by36,
		Size80by45
	}

	[Export] public StandardSizes StandardSize{
		get{
			return standardSize;
		}
		set{
			standardSize = value;
			MapSize = ((int)value+1) * new Vector2I(16, 9);
		}
	}
	private StandardSizes standardSize = StandardSizes.Size48by27;
	[Export] public Vector2I MapSize // Map size in tiles
	{
		get
		{
			return mapSize;
		}
		set
		{
			mapSize = value;
			UpdateDebug();
		}
	}
	private Vector2I mapSize = new Vector2I(52, 30);
	[Export(PropertyHint.Range, "0, 128, 16, or_greater")] int CameraEdgeLeway
    {
        get
        {
            return cameraEdgeLeway;
        }
        set
        {
            cameraEdgeLeway = value;
			UpdateDebug();
        }
    }
	int cameraEdgeLeway = 64;
	
	int tileSize = 16;
	public Vector2 pixelMapSize; // Map size in Pixels
	(Vector2 TL, Vector2 TR, Vector2 BL, Vector2 BR) cornersCoords;
	Area2D teleportsCheck;

	public override void _Ready()
	{
		teleportsCheck = GetChildOrNull<Area2D>(0);
		if (teleportsCheck == null)
			GD.Print("Teleports check not found, continuing");
		UpdateDebug();
		if(Engine.IsEditorHint())return;
		GetParent<Map>().camera.SetLeeway(cameraEdgeLeway);
		//SetupCollisionReplicator();
	}

	public override void _PhysicsProcess(double delta)
	{
		#if TOOLS
			if (Engine.IsEditorHint()) return;
		#endif
		ProcessTeleporting();
	}
	
	void ProcessTeleporting()
    {
		if (teleportsCheck == null) return;
		List<Node2D> nonTeleportingBodies;
		List<Area2D> nonTeleportingAreas;
		nonTeleportingBodies = teleportsCheck.GetOverlappingBodies().ToList();
		nonTeleportingAreas = teleportsCheck.GetOverlappingAreas().ToList();

		foreach (Player player in Game.Instance.players)
		{
			if (!nonTeleportingBodies.Contains(player)) continue;
			nonTeleportingBodies.Remove(player);

			Vector2 boundsDir = GetOutOfBoundsDir(player.Position);
			if (boundsDir != Vector2.Zero)
			{
				player.Position -= boundsDir * pixelMapSize;
			}
		}
		foreach (Node2D projectile in GetTree().GetNodesInGroup("Projectiles"))
        {
			if (projectile is Area2D areaProjectile)
				if (!nonTeleportingAreas.Contains(areaProjectile)) continue;
				else nonTeleportingAreas.Remove(areaProjectile);
			else 
				if (!nonTeleportingBodies.Contains(projectile)) continue;
				else nonTeleportingBodies.Remove(projectile);
            Vector2 boundsDir = GetOutOfBoundsDir(projectile.Position);
			if (boundsDir != Vector2.Zero)
			{
				projectile.Position -= boundsDir * pixelMapSize;
			}
        }
    }
	public Vector2 GetOutOfBoundsDir(Vector2 position)
	{
		Vector2 boundsVec = Vector2.Zero;
		if (position.X > -cornersCoords.TL.X)
			boundsVec += Vector2.Right;
		else if (position.X < cornersCoords.TL.X)
			boundsVec += Vector2.Left;

		if (position.Y > -cornersCoords.TL.Y)
			boundsVec += Vector2.Down;
		else if (position.Y < cornersCoords.TL.Y)
			boundsVec += Vector2.Up;

		return boundsVec;
    }

	void UpdateDebug()
	{
		updateSizeVars();
		if (!Engine.IsEditorHint()) return;
		QueueRedraw();
	}

	public override void _Draw()
	{
		base._Draw();
		if (!IsNodeReady() || !drawBorders || !Engine.IsEditorHint())
			return;
		int lineThickness = 5;
		float halfLineThickness = (float)lineThickness / 2;

		Godot.Vector2[] cornersArray =
		{
			cornersCoords.TL + new Vector2(-halfLineThickness, -halfLineThickness),
			cornersCoords.TR + new Vector2(halfLineThickness, -halfLineThickness),
			cornersCoords.BR + new Vector2(halfLineThickness, halfLineThickness),
			cornersCoords.BL + new Vector2(-halfLineThickness, halfLineThickness),
			cornersCoords.TL + new Vector2(-halfLineThickness, -halfLineThickness),
		};

		Godot.Vector2[] cornersWithLeewayArray =
        [
            cornersCoords.TL + new Vector2(-halfLineThickness, -halfLineThickness) + new Vector2(-1, -1) * CameraEdgeLeway,
			cornersCoords.TR + new Vector2(halfLineThickness, -halfLineThickness) + new Vector2(1, -1) * CameraEdgeLeway,
			cornersCoords.BR + new Vector2(halfLineThickness, halfLineThickness) + new Vector2(1, 1) * CameraEdgeLeway,
			cornersCoords.BL + new Vector2(-halfLineThickness, halfLineThickness) + new Vector2(-1, 1) * CameraEdgeLeway,
			cornersCoords.TL + new Vector2(-halfLineThickness, -halfLineThickness) + new Vector2(-1, -1) * CameraEdgeLeway,
		];

		DrawPolyline(
			cornersArray,
			Color.Color8(0, 255, 0, 128/2),
			lineThickness
		);

		DrawPolyline(
			cornersWithLeewayArray,
			Color.Color8(255, 0, 255, 128/2),
			lineThickness
		);
	}

	private void updateSizeVars()
	{
		pixelMapSize = MapSize * tileSize;
		Vector2 halfPixelMapSize = pixelMapSize / 2;
		cornersCoords = (
		new Vector2(-halfPixelMapSize.X, -halfPixelMapSize.Y),  //TL
		new Vector2(halfPixelMapSize.X, -halfPixelMapSize.Y),   //TR
		new Vector2(-halfPixelMapSize.X, halfPixelMapSize.Y),   //BL
		new Vector2(halfPixelMapSize.X, halfPixelMapSize.Y)     //BR
		);
	}

	/* void SetupCollisionReplicator()
	{
		int collisionThickness = 100;
		Godot.Vector2[] dirsArray = new Godot.Vector2[4]
		{
			Vector2.Right,
			Vector2.Up,
			Vector2.Left,
			Vector2.Down
		};

		foreach (Vector2 dir in dirsArray)
		{
			//CollisionReplicator collisionReplicator = new CollisionReplicator(); LEARN HOW TO DO THIS CUZ IT WOULD BE COOL AS FUCK
			CollisionsReplicator collisionReplicator = collisionReplicatorScene.Instantiate<CollisionsReplicator>();
			collisionReplicator.main = GetParent<Map>();
			
			CollisionShape2D collision = new CollisionShape2D();
			RectangleShape2D shape = new RectangleShape2D();
			collision.Shape = shape;
			collision.Position = dir * pixelMapSize / 2 + dir * collisionThickness / 2;
			if (dir.X != 0)
			{
				shape.Size = new Vector2(pixelMapSize.Y + collisionThickness * 2, collisionThickness);
				collision.Rotation = Mathf.Pi / 2;
			}
			else shape.Size = new Vector2(pixelMapSize.X + collisionThickness * 2, collisionThickness);

			collisionReplicator.side = dir;
			CallDeferred(MethodName.AddChild, collisionReplicator);
			collisionReplicator.CallDeferred(MethodName.AddChild, collision);
		}
	} */
}
