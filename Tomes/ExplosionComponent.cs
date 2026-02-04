using Godot;
using System;
using System.Collections;
using System.Drawing;
using System.Text.RegularExpressions;

public partial class ExplosionComponent : Area2D
{
	[Export] int frameLength = 3;
	[Export] CollisionShape2D explosionCollision;
	[Export] AudioStreamPlayer2D ExplosionAudio;
	public float explosionRadius;
	public int colorIdx = -1;
	CollisionShape2D shape;

	public override void _Ready()
	{
		AudioStreamPlayer2D newExplosionAudio = ExplosionAudio.Duplicate() as AudioStreamPlayer2D;

		Game.Instance.world.CallDeferred(MethodName.AddChild, newExplosionAudio);
		newExplosionAudio.Finished += () => newExplosionAudio.QueueFree();

		newExplosionAudio.GlobalPosition = GlobalPosition;
		newExplosionAudio.CallDeferred(AudioStreamPlayer2D.MethodName.Play);

		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
	}


	public void SetSize(float size)
	{
		explosionRadius = size;
		(explosionCollision.Shape as CircleShape2D).Radius = size;
	}
	void OnBodyEntered(Node2D body)
    {
		if (body is Player player)
		{
			if (hasLOS(player))
				player.TakeDamage(colorIdx);
		}
    }
	void OnAreaEntered(Area2D area)
	{
		if (area is DestructibleBlockFlag destructibleBlockFlag)
		{
			if (hasLOS(destructibleBlockFlag))
			{
				destructibleBlockFlag.Destroy();
			}
		}
    }
	bool hasLOS(Node2D losTo)
    {
        RayCast2D checkRay = new RayCast2D();
		checkRay.Position = Vector2.Zero;
		checkRay.TargetPosition = losTo.Position - Position;
		checkRay.SetCollisionMaskValue(2, true);
		checkRay.HitFromInside = true;
		AddChild(checkRay);
		return helpLOS(checkRay, losTo);
	}	
	bool helpLOS(RayCast2D checkRay, Node2D losTo)
    {      
		checkRay.ForceRaycastUpdate();
		var collider = checkRay.GetCollider();
		Vector2 collisionPoint = checkRay.GetCollisionPoint();
		
		if (!checkRay.IsColliding()) return false;

		if (collider is TileMapLayer tileMapLayer)
		{
			Vector2 losToPosToColPos = losTo.GlobalPosition - collisionPoint;
			if (Mathf.Abs(losToPosToColPos.X) <= 8 && Mathf.Abs(losToPosToColPos.Y) <= 8)
				return true;
		}
		if (collider == losTo)
			return true;
		else
			return false;
    }
    public override void _PhysicsProcess(double delta)
    {
		if (frameLength <= 0)
        {
            QueueFree();
        }
		else frameLength--;
    }
}
