using Godot;
using SpaceMages;
using System;
using System.Collections.Generic;

public partial class PoisonMine : Node2D
{
	[Export] Area2D detectionArea;
	[Export] Area2D collisionArea;
	[Export] PackedScene explosionComponentScene;
	ExplosionComponent explosionComponent;
	[Export] GpuParticles2D explosionParticles;
	GpuParticles2D newExplosionParticles;
	[Export] AnimatedSprite2D sprite;
	public Vector2 wallSide;
	public Player owner;
	const float FUSETIME = 0.5f;
	float CurrentFuseTime
    {
        get
        {
            return currentFuseTime;
        }
        set
        {
            currentFuseTime = value;
			GD.Print(value);
			if (value == 0) Explode();
        }
    }
	float currentFuseTime = 1f;
	float explosionRadius = -1;

	public override void _Ready()
	{
		base._Ready();
		detectionArea.BodyEntered += OnBodyEntered;
		collisionArea.AreaEntered += OnAreaCollided;
		explosionRadius = (GetNode<CollisionShape2D>("ExplosionRadius").Shape as CircleShape2D).Radius;

		PrepareExplosion();
		SetDirection();
		Game.Instance.NewRoundStarted += OnNewRoundStarted;
	}

	void OnNewRoundStarted()
	{
		QueueFree();
	}

    void PrepareExplosion()
	{
		newExplosionParticles = explosionParticles.Duplicate() as GpuParticles2D;
		newExplosionParticles.Finished += () => newExplosionParticles.QueueFree();
		ExplosionComponent explosionComponent = explosionComponentScene.Instantiate<ExplosionComponent>();
		explosionComponent.GlobalPosition = GlobalPosition + wallSide * 16; 
		explosionComponent.SetSize(explosionRadius);
		explosionComponent.colorIdx = owner.colorIdx;
		this.explosionComponent = explosionComponent;
	}

	void SetDirection()
    {
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(this, "position", Position + wallSide * 16, 1f);
    }

    public override void _Process(double delta)
    {
        sprite.Position = Vector2.Zero + new Vector2(GD.RandRange(-1, 1), GD.RandRange(-1, 1)).Normalized() * (1 - CurrentFuseTime)*2;
		sprite.Frame = (int) ((1 - CurrentFuseTime)*sprite.SpriteFrames.GetFrameCount("Explode") + 0.5f);
    }

	void OnBodyEntered(Node2D body)
    {
        if (body is not Player) return;

		Tween tween = CreateTween();
		tween.TweenProperty(this, "CurrentFuseTime", 0, FUSETIME);
    }

	private void OnAreaCollided(Area2D area)
	{
		if (area is not LinearProjectile) return;
		Explode();
	}

	void Explode()
    {
		explosionComponent.GlobalPosition = GlobalPosition;
		GD.Print(explosionComponent.GlobalPosition);
		SummonExplosionParticles();
	    Game.Instance.world.CallDeferred(MethodName.AddChild, explosionComponent);
		QueueFree();
    }
	void SummonExplosionParticles()
	{
		newExplosionParticles.GlobalPosition = GlobalPosition;
		newExplosionParticles.SetDeferred(GpuParticles2D.PropertyName.Emitting, true);
		Game.Instance.world.CallDeferred(MethodName.AddChild, newExplosionParticles);
	}

    public override void _ExitTree()
    {
        base._ExitTree();

		Game.Instance.NewRoundStarted -= OnNewRoundStarted;
    }

}
