using Godot;
using System;
using System.IO;

public partial class HitscanBullet : RayCast2D
{
	[Export] protected float lifeTime = 0.5f; 
	[ExportGroup("Debug")]
	[Export] protected bool printDebug = false;
	public Player owner;
	public Vector2 inputDir;
	protected float distance = -1;
	float maxLength = 10000f;
	Timer lifeTimer = new();


	public override void _Ready()
	{
		TargetPosition = new Vector2(1, 0) * maxLength;
		Rotation = inputDir.Angle();
		lifeTimer.OneShot = true;
		lifeTimer.Timeout += OnLifeEnd;
		AddChild(lifeTimer);
		lifeTimer.Start(lifeTime);
		CheckHit();
	}

    void CheckHit()
	{
		ForceRaycastUpdate();
		var collider = GetCollider();
		while (collider != null)
		{
			if (collider is Area2D)
			{
				if (CheckHitArea2D(collider as Area2D))
					break;
			}
			else
			{
				if (CheckHitBody(collider as Node2D))
					break;
			}
			GD.Print(collider);
			GD.Print((GetCollisionPoint() - GlobalPosition).Length());
			ForceRaycastUpdate();
			collider = GetCollider();
		}
		Enabled = false;
		if (collider == null)
		{
			distance = 500f;
		}
		else
		{
			distance = (GetCollisionPoint() - GlobalPosition).Length();
		}
	}
	
	bool CheckHitArea2D(Area2D collider)
	{
		if (collider is HittableComponent hittableComponent)
		{

			hittableComponent.Hit(this);

			if (!hittableComponent.stopsHitscan || !hittableComponent.Enabled)
			{
				AddException(collider);
				return false;
			}
			else return true;
		}
		else
		{
			AddException(collider);
			return false;
		}
	}
	bool CheckHitBody(Node2D collider)
	{
		if (collider is Player player)
		{
			if (player.colorIdx == owner.colorIdx)
            {
				AddException(player);
				return false;
            }
			else Hit(player);
		}
		else if (collider is TileMapLayer)
		{
			Hit(collider as TileMapLayer);
		}
		else if (collider is StaticBody2D)
		{
			Hit(collider as StaticBody2D);
		}
		return true;
	}
    protected virtual void Hit(Player player)
	{
		player.TakeDamage(owner);
	}
	protected virtual void Hit(StaticBody2D body)
	{
		GD.Print(body);
	}
	
	public virtual void Hit(TileMapLayer mapLayer)
	{
		if (mapLayer.GetParent() is not InteractableTiles parentLayer) return;
		
		Vector2I tilePos = GetTilePos(GetCollisionPoint(), mapLayer.TileSet.TileSize);
		parentLayer.destructibleBlockFlags[tilePos].Destroy();
	}
	Vector2I GetTilePos(Vector2 pos, Vector2 tileSize)
	{
		pos -= GetCollisionNormal();
		pos /= (int)tileSize.X;
		if (pos.X < 0) pos.X --;
		if (pos.Y < 0) pos.Y --;
		Vector2I tilePos = ToIntVec(pos);
		return tilePos;
	}	
	static Vector2I ToIntVec(Vector2 vec)
	{
		return new Vector2I(
			(int) vec.X,
			(int) vec.Y
		);
	}
    private void OnLifeEnd()
    {
		End();
    }
	public void End()
    {
		QueueFree();
    }
}
