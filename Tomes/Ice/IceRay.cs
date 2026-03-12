using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Pipes;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

public partial class IceRay : HitscanBullet
{
	[Export] float freezeTime = 2f;
	[ExportGroup("Visual")]
	[Export] int animationSpeed = 1;
	[ExportGroup("Nodes")]
	[Export] public GpuParticles2D rayRingEmitter;
	[Export] public GpuParticles2D iceDroppletsEmitter;
	[Export] public GpuParticles2D hitEmitter;
	float distance = -1;
	Timer iceParticlesCoyoteTimer = new();
	float iceParticlesCoyoteTime = 1;
	List<GpuParticles2D> iceParticlesQueue = [];
	Node2D iceDroppletsParent = new();
	Node2D rayRingsParent = new();
	List<(GpuParticles2D node, float velocity)> fallingIce = [];

	public override void _Ready()
	{
 		base._Ready();
		AddParticleParents();
		CheckHit();
		CallDeferred(MethodName.SpawnParticleEmitters);
 	}

    public override void _PhysicsProcess(double delta)
	{
		for(int i = 0; i < fallingIce.Count; i++)
		{
			(GpuParticles2D node, float velocity) = fallingIce[i];
			node.Position += Vector2.Down * velocity;
			velocity += (float)delta;
			fallingIce[i] = (node, velocity);
		}
	}

	void AddParticleParents()
	{
		AddChild(iceDroppletsParent);
		AddChild(rayRingsParent);
		iceDroppletsEmitter.Reparent(iceDroppletsParent);
		iceParticlesQueue.Add(iceDroppletsEmitter);
		rayRingEmitter.Reparent(rayRingsParent);
	}

	void SpawnParticleEmitters()
	{
		if (distance == -1)
		{
			Vector2 collidingPoint = GetCollisionPoint();
			distance = (collidingPoint - GlobalPosition).Length();
		}
		Enabled = false;
		hitEmitter.Position = new Vector2(distance, 0);
		InitializeSpawnEmitters();
		AnimateIceParticlesEmitter();
		iceDroppletsEmitter.OneShot = true;
		iceDroppletsEmitter.Emitting = true;
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
		FellIceDropplet();
	}

	async void FellIceDropplet()	
	{
		if (iceParticlesQueue.Count <= 0)
		{
			return;
		}
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		fallingIce.Add((iceParticlesQueue[0], 0));
		iceParticlesQueue.Remove(iceParticlesQueue[0]);
		
		FellIceDropplet();
	}

	void InitializeSpawnEmitters()
	{
		rayRingEmitter.OneShot = true;
		rayRingEmitter.Emitting = true;
		int separation = (int) rayRingEmitter.Position.X;
		SpawnRayRings(separation);
	}
	async void SpawnRayRings(int separation, int animationStep = 1, int current = 2)
	{
		if (separation * current > distance)
		{
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
		(GpuParticles2D newRayRing, GpuParticles2D newIceParticles) = GPUParticlesPool.GetIceParticles();


		newRayRing.OneShot = true;
		newRayRing.Position = GlobalPosition + (current * separation * Vector2.Right.Rotated(Rotation));
		newRayRing.Emitting = true;
		newRayRing.Rotation = Rotation;
	
		newIceParticles.OneShot = true;
		newIceParticles.Position = GlobalPosition + (current * separation * Vector2.Right.Rotated(Rotation));
		newIceParticles.Emitting = true;
		newIceParticles.Rotation = Rotation;
		iceParticlesQueue.Add(newIceParticles);
		if (printDebug) GD.Print("spawned particles ", current);
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
	protected override void Hit(Player player)
	{
		if(player.HasShield)
			player.TakeDamage(owner);
		else
			player.effectHandler.Freeze();
	}
    protected override void Hit(StaticBody2D body)
    {
		base.Hit(body);
    }
	
	void Hit(TileMapLayer mapLayer)
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
	Vector2I ToIntVec(Vector2 vec)
	{
		return new Vector2I(
			(int) vec.X,
			(int) vec.Y
		);
	}

    public override void _ExitTree()
    {
        base._ExitTree();
		if (printDebug) GD.Print("Distance: ", distance, " | ","Hitting: ", (GetCollider() as Node).Name, " | ", "Hit Pos: ", GetCollisionPoint(), " | ", "Global Position: ", GlobalPosition);
		foreach(GpuParticles2D particles in rayRingsParent.GetChildren())
		{
			if (particles.Name.ToString().StartsWith("GPUParticles2D")) continue;
			GPUParticlesPool.ReturnRing(particles);
		}

		foreach(GpuParticles2D particles in iceDroppletsParent.GetChildren())
		{
			if (particles.Name.ToString().StartsWith("GPUParticles2D")) continue;
			GPUParticlesPool.ReturnDropplet(particles);
		}
    }

}