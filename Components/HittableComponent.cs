using Godot;
using Godot.NativeInterop;
using System;
using System.Linq.Expressions;
using System.Threading;

public partial class HittableComponent : Area2D
{
	[Signal] public delegate void GotHitEventHandler(Node2D hitter);
	
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
	[Export] public bool blocksExplosions = false;

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
			GetHit(meleeAttack.Main);
		}
		else if (area is LinearProjectile projectile && linearProjectilesAllowed)
		{
			projectile.End();
			GetHit(projectile);
		}
		else if (area is ExplosionComponent explosionComponent && explosionAllowed)
		{
			GetHit(explosionComponent);
		}
	}	

	public void Hit(Node2D Hitter)
	{
		if (!enabled) return;
		if (Hitter is HitscanBullet ray && hitscanBulletAllowed)
		{
			GetHit(ray);
		}
		else if (Hitter is MeleeAttack meleeAttack && meleeAllowed)
		{
			GetHit(meleeAttack.Main);
		}
		else if (Hitter is LinearProjectile projectile && linearProjectilesAllowed)
		{
			projectile.End();
			GetHit(projectile);
		}
	}

	void GetHit(Node2D Hitter)
	{
		EmitSignal(SignalName.GotHit, Hitter);
	}
}
