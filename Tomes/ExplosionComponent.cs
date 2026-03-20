using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.XPath;

public partial class ExplosionComponent : Area2D
{
	[Export] int frameLength = 3;
	[Export] CollisionShape2D explosionCollision;
	[Export] AudioStreamPlayer2D ExplosionAudio;
	public float explosionRadius;
	public Player owner;
	public int colorIdx = -1;
	CollisionShape2D shape;
	RayCast2D checkRay = new();

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		BodyEntered += OnBodyEntered;

		AudioStreamPlayer2D newExplosionAudio = ExplosionAudio.Duplicate() as AudioStreamPlayer2D;

		AddChild(checkRay);
		checkRay.AddException(this);
		checkRay.CollideWithAreas = true;
		checkRay.SetCollisionMaskValue(2, true);
		checkRay.HitFromInside = true;

		Game.Instance.world.CallDeferred(MethodName.AddChild, newExplosionAudio);
		newExplosionAudio.Finished += () => newExplosionAudio.QueueFree();

		newExplosionAudio.GlobalPosition = GlobalPosition;
		newExplosionAudio.CallDeferred(AudioStreamPlayer2D.MethodName.Play);
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
			if (HasLOS(player))
				if (owner != null)
					player.TakeDamage(owner);
				else if (colorIdx != -1)
					player.TakeDamage(colorIdx);
				else
					player.TakeDamage();
		}
    }
	void OnAreaEntered(Area2D area)
	{
		GD.Print("Checking: ", area, "at ", area.GlobalPosition);
		if (area is DestructibleBlockFlag destructible)
		{
			if (HasLOS(destructible))
				destructible.Destroy();
		}
	}
	bool HasLOS(Node2D losTo)
    {
		GD.Print("checking LOS to: ", losTo);
		foreach(Node child in losTo.GetChildren())
		{
			if (child is CollisionShape2D colShape && colShape.Shape is RectangleShape2D)
			{
				return CheckRectCollision(losTo, colShape);
			}
		}
		checkRay.TargetPosition = losTo.Position - Position;
		return CheckLOS(losTo);
	}	
	bool CheckRectCollision(Node2D losTo, CollisionShape2D colShape)
	{
		Vector2 shapeSize = (colShape.Shape as RectangleShape2D).Size;
		for(float Y = -1; Y <= 1; Y++)
		{
			for(float X = -1; X <= 1; X++)
			{
				Vector2 checkPosOffset = new Vector2(Y, X) * shapeSize/2;
				checkRay.TargetPosition = colShape.GlobalPosition + checkPosOffset.Rotated(colShape.Rotation) - Position;
				if (CheckLOS(losTo)) 
					return true;
			}
		}
		return false;
	} 
	bool CheckLOS(Node2D losTo)
    {
		if (losTo is InteractableTiles) return false;
		checkRay.ForceRaycastUpdate();
		if (!checkRay.IsColliding()) return true;

		var collider = checkRay.GetCollider();
		GD.Print("collider found: ", collider);
		//Vector2 collisionPoint = checkRay.GetCollisionPoint();

		if (collider != losTo)
		{
			if (collider is Player player){
				checkRay.AddException(player);
				return CheckLOS(losTo);
			}
			else if (collider is PlayeronPlayerCollision playerplayercol)
			{
				checkRay.AddException(playerplayercol);
				return CheckLOS(losTo);
			}
			else if (collider is HittableComponent hittableComponent && !hittableComponent.blocksExplosions)
			{
				checkRay.AddException(hittableComponent);
				return CheckLOS(losTo);
			}
			else if (collider is DestructibleBlockFlag destructibleFlag)
			{
				checkRay.AddException(destructibleFlag);
				return CheckLOS(losTo);
			}
			return false;
		}
		else return true;
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
