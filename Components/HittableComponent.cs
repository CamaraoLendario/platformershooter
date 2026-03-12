using Godot;
using Godot.NativeInterop;
using System;
using System.Linq.Expressions;
using System.Threading;

public partial class HittableComponent : Area2D
{
	[Signal] public delegate void GotHitEventHandler();
	[Signal] public delegate void GotHitidxEventHandler(int hitterIdx);
	[Signal] public delegate void GotHitvecEventHandler(Vector2 hitterIdx);
	

	[Export] public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
			if (enabled)
			{
				ForceCheck();
			}
		}
	}
	private bool enabled = false;

	[Export] bool breaksBullets = true;
	[Export] public bool stopsHitscan = false;

	[ExportGroup("Hittable by")]
	[Export] public bool meleeAllowed = true;
	[Export] public bool linearProjectilesAllowed = true;
	[Export] public bool hitscanBulletAllowed = true;
	[Export] public bool explosionAllowed = true;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

	void ForceCheck()
	{
		foreach(Area2D area in GetOverlappingAreas())
		{
			OnAreaEntered(area);
		}
	}

	void OnAreaEntered(Area2D area)
	{
		GD.Print("area entered!!!");
		if (!enabled) return;
		if (area is MeleeAttack meleeAttack && meleeAllowed)
		{
			GD.Print("melee entered!!!");
			GetHit(meleeAttack.GetIDX());
			GetHit(meleeAttack.GlobalPosition);
			GetHit();
		}
		else if (area is LinearProjectile projectile && linearProjectilesAllowed)
		{
			projectile.End();
			GetHit(projectile.owner.inputIdx);
			GetHit(projectile.GlobalPosition);
			GetHit();
		}
		else if (area is ExplosionComponent explosionComponent && explosionAllowed)
		{
			GetHit(explosionComponent.owner.inputIdx);
			GetHit(explosionComponent.GlobalPosition);
			GetHit();
		}
	}	

	public void Hit(Node2D Hitter)
	{
		if (!enabled) return;
		if (Hitter is HitscanBullet ray && hitscanBulletAllowed)
		{
			GetHit(ray.owner.inputIdx);
			GetHit(ray.GlobalPosition);
			GetHit();
		}
		else if (Hitter is MeleeAttack meleeAttack && meleeAllowed)
		{
			GetHit(meleeAttack.GetIDX());
			GetHit(meleeAttack.GlobalPosition);
			GetHit();
		}
		else if (Hitter is LinearProjectile projectile && linearProjectilesAllowed)
		{
			projectile.End();
			GetHit(projectile.owner.inputIdx);
			GetHit(projectile.GlobalPosition);
			GetHit();
		}
	}

	void GetHit()
	{
		EmitSignal(SignalName.GotHit);
	}

	void GetHit(int hitterIdx)
	{
		EmitSignal(SignalName.GotHitidx, hitterIdx);
	}
	
	void GetHit(Vector2 hitterPos)
	{
		GD.Print("got hit vector");
		EmitSignal(SignalName.GotHitvec, hitterPos);
	}
}
