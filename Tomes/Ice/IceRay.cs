using Godot;
using System;
using System.ComponentModel;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Threading.Tasks;

public partial class IceRay : HitscanBullet
{
	[Export] float freezeTime = 2f;
	[ExportGroup("Nodes")]
	[Export] GpuParticles2D iceParticlesEmitter;
	[Export] GpuParticles2D hitEmitter;
	[Export] GpuParticles2D rayRing;
	float distance;
	
	Vector2 originalIceParticlesEmitterPosition;
	Timer iceParticlesCoyoteTimer = new();
	float iceParticlesCoyoteTime = 1;

	public override void _Ready()
	{
 		base._Ready();
		checkHit();
		SpawnParticleEmitters();
 	}

	void SpawnParticleEmitters()
	{
		Vector2 collidingPoint = GetCollisionPoint();
		distance = (collidingPoint - GlobalPosition).Length();

		hitEmitter.Position = new Vector2(distance, 0);
		SpawnRayRings();
		AnimateIceParticlesEmitter();
		iceParticlesEmitter.OneShot = true;
		iceParticlesEmitter.Amount = (int)distance/2;
		iceParticlesEmitter.Emitting = true;
		hitEmitter.OneShot = true;
		hitEmitter.Emitting = true;
	}

	void AnimateIceParticlesEmitter()
	{
		ParticleProcessMaterial iceParticlesMaterial = (iceParticlesEmitter.ProcessMaterial.Duplicate() as ParticleProcessMaterial);
		originalIceParticlesEmitterPosition = iceParticlesEmitter.Position;
		iceParticlesMaterial.EmissionBoxExtents = new Vector3(distance/2, 2, 0);
		iceParticlesMaterial.EmissionShapeOffset = new Vector3(distance/2f, 0, 0);
		iceParticlesEmitter.ProcessMaterial = iceParticlesMaterial;
		
		iceParticlesCoyoteTimer.OneShot = true;
		AddChild(iceParticlesCoyoteTimer);
		iceParticlesCoyoteTimer.CallDeferred(Timer.MethodName.Start, iceParticlesCoyoteTime);

		iceParticlesCoyoteTimer.Timeout += () =>
		{
			float gravityAngle = Mathf.Pi/2 - Rotation;
			Vector2 gravity2D = new Vector2(Mathf.Cos(gravityAngle), Mathf.Sin(gravityAngle)) * 50;
			iceParticlesMaterial.Gravity = new Vector3(gravity2D.X, gravity2D.Y, 0);
		};
	}	

	void SpawnRayRings()
	{
		rayRing.OneShot = true;
		rayRing.Emitting = true;
		int separation = (int) rayRing.Position.X;
		for(int i = 2; i < (int) distance/separation; i++)
		{
			GpuParticles2D newRayRing = rayRing.Duplicate() as GpuParticles2D;

			newRayRing.Position = i * separation * Vector2.Right;
			AddChild(newRayRing);
		}
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		ShakeIceParticles();
    }

	void ShakeIceParticles()
	{
		if (iceParticlesCoyoteTimer.IsStopped()) return;

		Vector2 randVec = new Vector2((GD.Randf()*2)-1, (GD.Randf()*2)-1).Normalized();
		randVec *= (float)iceParticlesCoyoteTimer.TimeLeft / iceParticlesCoyoteTime;
		iceParticlesEmitter.Position = originalIceParticlesEmitterPosition + randVec;
	}

    void checkHit()
	{
		ForceRaycastUpdate();
		var collider = GetCollider();
		if (collider is Player player)
		{
			if (player.colorIdx == owner.colorIdx)
            {
				AddException(player);
				checkHit();
				return;
            }
			else Hit(player);
		}
		else if (collider is StaticBody2D)
		{
			Hit(collider as StaticBody2D);
		}

	}

	protected override void Hit(Player player)
	{
		GD.Print(player);
		if(player.HasShield)
			player.TakeDamage(owner);
		else
			player.effectHandler.Freeze(freezeTime);
	}
    protected override void Hit(StaticBody2D body)
    {
		base.Hit(body);
		GD.Print(body);
    }
}