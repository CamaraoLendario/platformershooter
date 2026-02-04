using Godot;
using System;

public partial class FireProjectile : LinearProjectile
{
	[Export] PackedScene explosionComponentScene;
	[Export] float explosionRadius = 100;
	[Export] GpuParticles2D trailParticles;
	[Export] GpuParticles2D explosionParticles;
	GpuParticles2D newExplosionParticles;
	[Export] AudioStreamPlayer2D FireAudio;
	Timer waitParticlesTimer = new();
	ExplosionComponent explosionComponent;


    public override void _Ready()
    {
		base._Ready();
		waitParticlesTimer.OneShot = true;
		AddChild(waitParticlesTimer);
		FireAudio.Finished += () => {FireAudio.Play();};
		waitParticlesTimer.Timeout += OnParticlesFinished;
		PrepareExplosion();
    }

	void PrepareExplosion()
	{
		newExplosionParticles = explosionParticles.Duplicate() as GpuParticles2D;
		newExplosionParticles.Finished += () => newExplosionParticles.QueueFree();
		ExplosionComponent explosionComponent = explosionComponentScene.Instantiate<ExplosionComponent>();
		explosionComponent.SetSize(explosionRadius);
		explosionComponent.colorIdx = owner.colorIdx;
		this.explosionComponent = explosionComponent;
	}

	protected override void OnBodyHit(Node2D body)
	{
		if (body is TileMapLayer || body is Player player && player.colorIdx != owner.colorIdx)
		{
			End();
        }
	}

	public override void End()
	{
		explosionComponent.Position = Position;
		Game.Instance.world.CallDeferred(MethodName.AddChild, explosionComponent);
		SummonExplosionParticles();
		sprite.Hide();
		trailParticles.Emitting = false;
		waitParticlesTimer.Start(trailParticles.Lifetime);
		SetDeferred(PropertyName.Monitorable, false);
		SetDeferred(PropertyName.Monitoring, false);
		velocity *= 0;
	}
	void SummonExplosionParticles()
	{
		newExplosionParticles.GlobalPosition = GlobalPosition;
		newExplosionParticles.SetDeferred(GpuParticles2D.PropertyName.Emitting, true);
		Game.Instance.world.CallDeferred(MethodName.AddChild, newExplosionParticles);

	}

	void OnParticlesFinished()
	{
        base.End();
    }
}
