using Godot;
using System;

public partial class FireProjectile : LinearProjectile
{
	[Export] PackedScene explosionComponentScene;
	[Export] float explosionRadius = 100;
	[Export] GpuParticles2D trailParticles;
	[Export] GpuParticles2D explosionParticles;
	[Export] AudioStreamPlayer2D FireAudio;
	Timer waitParticlesTimer = new();
	ExplosionComponent explosionComponent;
	float acceleration = 300f;


    public override void _Ready()
    {
		base._Ready();
		waitParticlesTimer.OneShot = true;
		AddChild(waitParticlesTimer);
		FireAudio.Finished += () => {FireAudio.Play();};
		waitParticlesTimer.Timeout += OnParticlesFinished;
		PrepareExplosionParticles();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		speed += acceleration * (float)delta;
    }


	void PrepareExplosionParticles()
	{
		explosionParticles.Finished += () => explosionParticles.QueueFree();
		explosionParticles.Reparent(Game.Instance.world);

		ExplosionComponent explosionComponent = explosionComponentScene.Instantiate<ExplosionComponent>();
		explosionComponent.SetSize(explosionRadius);
		explosionComponent.owner = owner;
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
		speed = 0;
	}

	void SummonExplosionParticles()
	{
		explosionParticles.GlobalPosition = GlobalPosition;
		explosionParticles.Emitting = true;
	}

	void OnParticlesFinished()
	{
        base.End();
    }
}
