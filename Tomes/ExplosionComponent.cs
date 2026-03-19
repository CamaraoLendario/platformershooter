using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

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
		checkRay.TargetPosition = losTo.Position - Position;
		GD.Print("checking LOS to: ", losTo);
		return HelpLOS(losTo);
	}	
	bool HelpLOS(Node2D losTo)
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
				return HelpLOS(losTo);
			}
			else if (collider is PlayeronPlayerCollision playerplayercol)
			{
				checkRay.AddException(playerplayercol);
				return HelpLOS(losTo);
			}
			else if (collider is HittableComponent hittableComponent && !hittableComponent.blocksExplosions)
			{
				checkRay.AddException(hittableComponent);
				return HelpLOS(losTo);
			}
			else if (collider is DestructibleBlockFlag destructibleFlag)
			{
				checkRay.AddException(destructibleFlag);
				return HelpLOS(losTo);
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
