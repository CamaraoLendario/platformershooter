using Godot;
using System;

public partial class PoisonProjectile : LinearProjectile
{
	World world;
	float groundLifetime = 3.0f;
	bool IsStuck
    {
        get
        {
            return isStuck;
        }
        set
        {
            isStuck = value;
			if(value)
				velocity *= 0;
        }
    }
	bool isStuck = false;
	public override void _Ready()
	{
		world = Game.Instance.world;
		base._Ready();
	}

	protected override void OnBodyHit(Node2D body)
	{
		if (IsStuck) return;
		if (body is TileMapLayer || body is StaticBody2D) OnWallCollision();
		if (body is Player player) OnPlayerCollision(player);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (IsStuck) return;

		Vector2 velDir = velocity.Normalized();
		if (velDir.Y >= 0)
			Rotation = MathF.Acos(velDir.X);
		else Rotation = -MathF.Acos(velDir.X);


		isInPilotArea = world.currentMap.IsPositionInPilotArea(GlobalPosition);
		velocity -= velocity.Normalized() * speed * (float)delta;
		if (isInPilotArea) velocity.Y += speed * 3 * (float)delta;
		else if (velocity.LengthSquared() < 0.1f) velocity *= 0;
	}
    public override void OnLifeEnd()
    {
		QueueFree();
    }
	public void OnWallCollision()
	{
		Land();
		CollisionConfirm();
		lifeTimer.Start(groundLifetime);
	}
	
	void OnPlayerCollision(Player player)
	{
		if (player.colorIdx == owner.colorIdx) return;
		Land();
		lifeTimer.Stop();

		CallDeferred(MethodName.Reparent, player, true);
		player.effectHandler.AddPoisonBlob(this);
    }
	
	void Land()
    {
        SetDeferred(PropertyName.Monitoring, false);
		SetDeferred(PropertyName.Monitorable, false);
		IsStuck = true;
		velocity *= 0;
    }

	public async void RemoveFromPlayer()
    {
		velocity = Position.Normalized() * speed;
		IsStuck = false;
		CallDeferred(MethodName.Reparent, Game.Instance.world, true);
		await ToSignal(GetTree().CreateTimer(0.5f), Timer.SignalName.Timeout);
        QueueFree();
    }
}
