using Godot;
using System;
using System.Collections.Generic;

public partial class LinearProjectile : Area2D
{
	[Export] public float speed = 500f;
	[Export] public float lifeTime = 10f;
	[Export] bool isDestroyedOutOfZone = true;
	[ExportGroup("Nodes")]
	[Export] public Node2D sprite;
	[Export] Node2D confirmRaysNode;
	public Vector2 direction;
	public Vector2 velocity;
	public Player owner;
	public bool isInPilotArea = true;
	protected float collisionConfirmLength = 24.0f;
	protected Timer lifeTimer = new Timer();
	bool used = false;

	public override void _Ready()
	{
		velocity = direction * speed;
		if (direction.Y >= 0)
			sprite.Rotation = MathF.Acos(direction.X);
		else sprite.Rotation = -MathF.Acos(direction.X);
	
		if (sprite.Rotation > Mathf.Pi/2 || sprite.Rotation < -Mathf.Pi/2)
			FlipVSprite(sprite, true);


		lifeTimer.OneShot = true;
		AddChild(lifeTimer);
		lifeTimer.Timeout += OnLifeEnd;
		BodyEntered += OnBodyHit;
		lifeTimer.Start(lifeTime);
		if (isDestroyedOutOfZone) Game.Instance.BulletsNodes.Add(this);

		Game.Instance.NewRoundStarted += OnRoundFinished;
		AreaEntered += OnAreaEntered;
	}

    private void OnAreaEntered(Area2D area)
    {
		GD.Print("Area found: " + area);
		if (area is not DestructibleBlockFlag destructibleBlockFlag) return;
		GD.Print("Destroying " + area);
		destructibleBlockFlag.Destroy();
    }

    public override void _PhysicsProcess(double delta)
	{
		Position += velocity * (float)delta;
	}

	public void SetDirection(float inputRotation)
	{
		if (inputRotation < 0)
		{
			inputRotation += (2 * MathF.PI);
		}
		direction = new Vector2(MathF.Cos(inputRotation), MathF.Sin(inputRotation));
	}
	public void SetDirection(Vector2 inputVector)
	{
		direction = inputVector.Normalized();
	}
	public void SetDirection(int x, int y)
	{
		direction = new Vector2(x, y).Normalized();
	}

	protected virtual void OnBodyHit(Node2D body)
	{
		if (used) return;
		if (body is TileMapLayer tileMapLayer) End(tileMapLayer);
		if (!(body is Player) || ((body is Player) && ((body as Player).colorIdx == owner.colorIdx))) return;

		Player player = body as Player;

		player.TakeDamage(owner);
		used = true;
		QueueFree();
	}
	
	public virtual void OnLifeEnd()
    {
		End();
    }

	public virtual void End(TileMapLayer tileMapLayer)
	{
		foreach(Area2D area in GetOverlappingAreas())
        {
			if (area is not DestructibleBlockFlag destructibleBlockFlag) continue;
			
			destructibleBlockFlag.Destroy();
        }

		End();
	}
	public virtual void End()
	{
		QueueFree();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Game.Instance.NewRoundStarted -= OnRoundFinished;
		if (isDestroyedOutOfZone) Game.Instance.BulletsNodes.Remove(this);
	}

	protected virtual void CollisionConfirm()
	{
		if (IsQueuedForDeletion()) return;
		List<Vector2> CollisionPoints = [
			Vector2.Zero,
			Vector2.Zero,
			Vector2.Zero,
		];

		for (int i = -1; i <= 1; i++)
		{
			RayCast2D confirmRay = confirmRaysNode.GetChild<RayCast2D>(i);

			confirmRay.Position = Vector2.Left.Rotated((Mathf.Pi / 2) * i) * collisionConfirmLength;
			confirmRay.TargetPosition = Vector2.Right.Rotated((Mathf.Pi / 2) * i) * collisionConfirmLength * 2;

			confirmRay.ForceRaycastUpdate();
			Vector2 collisionPos = confirmRay.GetCollisionPoint();
			if (collisionPos != confirmRay.GlobalPosition)
				CollisionPoints[i + 1] = collisionPos;
		}

		var tempPos = CollisionPoints;
		CollisionPoints[0] = tempPos[1];
		CollisionPoints[1] = tempPos[0];

		for (int i = 0; i < CollisionPoints.Count - 1; i++)
        {
			if (CollisionPoints[i] != Vector2.Zero)
			{
				GlobalPosition = CollisionPoints[i];
				
				return;
			}
        }
	}
	
    public virtual void OnRoundFinished()
    {
		QueueFree();
    }


	void FlipVSprite(Node2D sprite, bool flip)
	{
		if(sprite is Sprite2D Sprite)
		{
			Sprite.FlipV = flip;
		}
		else if (sprite is AnimatedSprite2D animatedSprite)
			animatedSprite.FlipV = flip;
	}
}
