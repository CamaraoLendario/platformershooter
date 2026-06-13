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
	World world;

    public override void _Ready()
    {
		base._Ready();
		waitParticlesTimer.OneShot = true;
		AddChild(waitParticlesTimer);
		FireAudio.Finished += () => {FireAudio.Play();};
		waitParticlesTimer.Timeout += OnParticlesFinished;
		world = GetTree().GetFirstNodeInGroup("World") as World;
		PrepareExplosionParticles();
    }

    public override void _PhysicsProcess(double delta)
    {
		if (ending) return;
        base._PhysicsProcess(delta);
		speed += acceleration * (float)delta;
    }


	void PrepareExplosionParticles()
	{
		explosionParticles.Finished += () => explosionParticles.QueueFree();
		explosionParticles.Reparent(world);

		ExplosionComponent explosionComponent = explosionComponentScene.Instantiate<ExplosionComponent>();
		explosionComponent.SetSize(explosionRadius);
		explosionComponent.owner = owner;
		this.explosionComponent = explosionComponent;
	}

	protected override void OnBodyHit(Node2D body)
	{
		if (body is Player player && player.colorIdx != owner.colorIdx)
			End(EndingReason.HITPLAYER);
		else if (body is TileMapLayer)
			End(EndingReason.HITGEOMETRY);
	}

	public override bool End(EndingReason endingReason, bool allowFreeing = true, bool forceFreeing = false)
	{
		if (!base.End(endingReason, false)) return false;
		explosionComponent.Position = Position;
		world.CallDeferred(MethodName.AddChild, explosionComponent);
		SummonExplosionParticles();
		waitParticlesTimer.Start(trailParticles.Lifetime);
		ending = true;
		SetDeferred(PropertyName.Monitorable, false);
		SetDeferred(PropertyName.Monitoring, false);
		speed = 0;
		PrepareForDeletion();
		return true;
	}

	async void PrepareForDeletion()
	{
		trailParticles.Emitting = false;
		sprite.SelfModulate = new Color(0f, 0f, 0f, 0f);
		FireAudio.Stop();
		await ToSignal(GetTree().CreateTimer(trailParticles.Lifetime), Timer.SignalName.Timeout);
		QueueFree();
	}

    void SummonExplosionParticles()
	{
		explosionParticles.GlobalPosition = GlobalPosition;
		explosionParticles.Emitting = true;
	}

	void OnParticlesFinished()
	{
        base.End(EndingReason.TIMEOUT);
    }
}
