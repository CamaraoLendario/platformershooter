using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Pipes;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Threading.Tasks;

public partial class IceRay : HitscanBullet
{
	[Export] float freezeTime = 2f;
	[ExportGroup("Visual")]
	[Export] int animationSpeed = 1;
	[ExportGroup("Nodes")]
	[Export] GpuParticles2D iceParticlesEmitter;
	[Export] GpuParticles2D hitEmitter;
	[Export] GpuParticles2D rayRing;
	float distance;
	Timer iceParticlesCoyoteTimer = new();
	float iceParticlesCoyoteTime = 1;
	List<GpuParticles2D> iceParticlesQueue = new();
	Node2D iceDroppletsParent = new();
	Node2D rayRingsParent = new();

	public override void _Ready()
	{
 		base._Ready();
		AddParticleParents();
		checkHit();
		SpawnParticleEmitters();
 	}
	void AddParticleParents()
	{
		AddChild(iceDroppletsParent);
		iceParticlesEmitter.Reparent(iceDroppletsParent);
		iceParticlesQueue.Add(iceParticlesEmitter);
		AddChild(rayRingsParent);
		rayRing.Reparent(rayRingsParent);
	}

	void SpawnParticleEmitters()
	{
		Vector2 collidingPoint = GetCollisionPoint();
		distance = (collidingPoint - GlobalPosition).Length();

		hitEmitter.Position = new Vector2(distance, 0);
		InitializeSpawnEmitters();
		AnimateIceParticlesEmitter();
		iceParticlesEmitter.OneShot = true;
		iceParticlesEmitter.Emitting = true;
		hitEmitter.OneShot = true;
		hitEmitter.Emitting = true;
	}

	void AnimateIceParticlesEmitter()
	{	
		iceParticlesCoyoteTimer.OneShot = true;
		AddChild(iceParticlesCoyoteTimer);
		iceParticlesCoyoteTimer.CallDeferred(Timer.MethodName.Start, iceParticlesCoyoteTime);

		iceParticlesCoyoteTimer.Timeout += () =>
		{
			InitializeFellIceDropplets();
		};
	}

	void InitializeFellIceDropplets()
	{
		float gravityAngle = Mathf.Pi/2 - Rotation;
		Vector2 gravity2D = new Vector2(Mathf.Cos(gravityAngle), Mathf.Sin(gravityAngle)) * 50;
		FellIceDropplet(gravity2D);
	}

	async void FellIceDropplet(Vector2 gravity2D)	
	{
		if (iceParticlesQueue.Count <= 0)
		{
			GD.Print("finished felling ice particles");	
			return;
		}
		GD.Print("felling Ice particles");	
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		ParticleProcessMaterial iceParticlesMaterial = iceParticlesQueue[0].ProcessMaterial.Duplicate() as ParticleProcessMaterial;
		iceParticlesMaterial.Gravity = new Vector3(gravity2D.X, gravity2D.Y, 0);
		iceParticlesQueue[0].ProcessMaterial = iceParticlesMaterial;
		iceParticlesQueue.Remove(iceParticlesQueue[0]);
		
		FellIceDropplet(gravity2D);
	}

	void InitializeSpawnEmitters()
	{
		rayRing.OneShot = true;
		rayRing.Emitting = true;
		int separation = (int) rayRing.Position.X;
		SpawnRayRings(separation);
	}
	async void SpawnRayRings(int separation, int animationStep = 1, int current = 2)
	{
		if (separation * current > distance)
		{
			GD.Print("ended");
			return;
		}
		if (animationStep <= animationSpeed)
		{
			animationStep ++;
		}
		else
		{
			animationStep = 1;
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		}
		
		GD.Print("summoned ring " + current);
		GpuParticles2D newRayRing = rayRing.Duplicate() as GpuParticles2D;
		GpuParticles2D newIceParticles = iceParticlesEmitter.Duplicate() as GpuParticles2D;
		
		newRayRing.OneShot = true;
		newRayRing.Emitting = true;
		newRayRing.Position = current * separation * Vector2.Right;
		rayRingsParent.AddChild(newRayRing);

		newIceParticles.Amount = 1;
		newIceParticles.OneShot = true;
		newIceParticles.Emitting = true;
		newIceParticles.Position = current * separation * Vector2.Right;
		iceDroppletsParent.AddChild(newIceParticles);
		iceParticlesQueue.Add(newIceParticles);
		
		SpawnRayRings(separation, animationStep, current+1);
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
		iceDroppletsParent.Position = Vector2.Zero + randVec;
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