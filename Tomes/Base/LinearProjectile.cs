using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

public partial class LinearProjectile : Area2D
{
	[Export] public float speed = 500f;
	[Export] public float lifeTime = 10f;
	[Export] bool isDestroyedOutOfZone = true;
	[Flags]
	public enum ReasonFlags
	{
		EndsToDestructible = 1 << 0,
		EndsToGeometry = 1 << 1,
		EndsToPlayer = 1 << 2,
		TimesOut = 1 << 3,
	}

	[Export]
	public ReasonFlags CanEndBy = (ReasonFlags)15;
	[ExportGroup("Nodes")]
	[Export] public Node2D sprite;
	[Export] Node2D confirmRaysNode;

	public Vector2 Direction
	{
		get
		{
			return direction;
		}
		set
		{
			direction = value;
			sprite.Rotation = value.Angle();
		}
	}
	private Vector2 direction;
	
	PilotArea pilotArea;
	public Player owner;
	public bool isInPilotArea = true;
	protected float collisionConfirmLength = 24.0f;
	protected Timer lifeTimer = new Timer();
	protected bool ending = false;
	protected bool isColiding = false;
	public enum EndingReason
	{
		HITDESTRUCTIBLE,
		HITGEOMETRY,
		HITPLAYER,
		TIMEOUT,
	}
	
	public override void _Ready()
	{
		sprite.Rotation = Direction.Angle();
	
		if (sprite.Rotation > Mathf.Pi/2 || sprite.Rotation < -Mathf.Pi/2)
			FlipVSprite(sprite, true);

		lifeTimer.OneShot = true;
		AddChild(lifeTimer);
		lifeTimer.Timeout += OnLifeEnd;
		BodyEntered += OnBodyHit;
		lifeTimer.Start(lifeTime);
		
		pilotArea = Game.GetMap().GetNode<PilotArea>("%PilotArea");
		isInPilotArea = pilotArea.IsInPilotArea(Position);
		SignalBus.Instance.RoundFinished += OnRoundFinished;

		AreaExited += (Area2D area) =>
		{
			if (Monitoring)
			if (!HasOverlappingAreas() && !HasOverlappingBodies()) isColiding = false;
		};
		BodyExited += (Node2D area) =>
		{
			if (Monitoring)
			if (!HasOverlappingAreas() && !HasOverlappingBodies()) isColiding = false;
		};
	}

    public override void _PhysicsProcess(double delta)
	{
		if (ending) return;
		Move(delta);
	}

	int checkCount = 0;
    protected void CheckForDestroyTiles()
    {
		if(!Monitoring) return;
		checkCount ++;
		if (!HasOverlappingAreas()) return;
		isColiding = true;
		foreach (Area2D overlappingArea in GetOverlappingAreas())
		{
			GD.Print("Area found: " + overlappingArea.Name);
			if (overlappingArea is DestructibleBlockFlag destructibleBlockFlag) {
				GD.Print("Destroying " + overlappingArea.Name);
				destructibleBlockFlag.Destroy();
				End(EndingReason.HITDESTRUCTIBLE);
			}
		}
    }
	
	protected virtual void Move(double delta)
	{
		Position += Direction * speed * (float)delta;
	}

    public void SetDirection(float inputRotation)
	{
		if (inputRotation < 0)
		{
			inputRotation += (2 * MathF.PI);
		}
		Direction = new Vector2(MathF.Cos(inputRotation), MathF.Sin(inputRotation));
	}
	public void SetDirection(Vector2 inputVector)
	{
		Direction = inputVector.Normalized();
	}
	public void SetDirection(int x, int y)
	{
		Direction = new Vector2(x, y).Normalized();
	}

	protected virtual void OnBodyHit(Node2D body)
	{
		if ((body is Player) && ((body as Player).colorIdx == owner.colorIdx)) return;
		isColiding = true;
		if (body is TileMapLayer tileMapLayer)
		{
			CallDeferred(MethodName.CheckForDestroyTiles);
			End(EndingReason.HITGEOMETRY);
			return;
		}
		if (!(body is Player)) return;

		Player player = body as Player;

		End(EndingReason.HITPLAYER);
		player.TakeDamage(owner);
	}
	
	public virtual void OnLifeEnd()
    {
		End(EndingReason.TIMEOUT);
    }
	public virtual bool End(EndingReason endingReason, bool allowFreeing = true, bool forceFreeing = false)
	{
		if (ending) return true;
		switch (endingReason)
		{
			case EndingReason.HITDESTRUCTIBLE:
				if ((CanEndBy & ReasonFlags.EndsToDestructible) != 0)
					ending = true;
				break;
			case EndingReason.HITGEOMETRY:
				if ((CanEndBy & ReasonFlags.EndsToGeometry) != 0)
					ending = true;
				break;
			case EndingReason.HITPLAYER:
				if ((CanEndBy & ReasonFlags.EndsToPlayer) != 0)
					ending = true;
				break;
			case EndingReason.TIMEOUT:
				if ((CanEndBy & ReasonFlags.TimesOut) != 0)
					ending = true;
				break;
		}
		
		if (forceFreeing)
			QueueFree();
		
		if (!ending) return false;
		//CheckForDestroyTiles();
		if (allowFreeing)
			QueueFree();
		return true;
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
    public override void _ExitTree()
    {
		SignalBus.Instance.RoundFinished -= OnRoundFinished;
    }

}
