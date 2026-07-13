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
	[Export] public AnimatedSprite2D sprite;
	public Vector2 direction {get; private set;}
	PilotArea pilotArea;
	public Player owner;
	public bool isInPilotArea = true;
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
		Position += direction * speed * (float)delta;
	}

    public void SetDirection(float inputRotation)
	{
		inputRotation = Mathf.Abs(inputRotation);
		SetDirection(new Vector2(MathF.Cos(inputRotation), MathF.Sin(inputRotation)));
	}
	public virtual void SetDirection(Vector2 inputVector)
	{
		direction = inputVector.Normalized();
		sprite.Rotation = direction.Angle();
	
		if (sprite.Rotation > Mathf.Pi/2 || sprite.Rotation < -Mathf.Pi/2)
			sprite.FlipV = true;
	}

	protected virtual void OnBodyHit(Node2D body)
	{
		if ((body is Player player) && player.GetTeam() == owner.GetTeam())
			return;
		isColiding = true;
		if (body is TileMapLayer tileMapLayer)
		{
			CallDeferred(MethodName.CheckForDestroyTiles);
			End(EndingReason.HITGEOMETRY);
			return;
		}
		if (!(body is Player)) return;

		End(EndingReason.HITPLAYER);
		(body as Player).TakeDamage(owner);
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
	
    public virtual void OnRoundFinished()
    {
		QueueFree();
    }

    public override void _ExitTree()
    {
		SignalBus.Instance.RoundFinished -= OnRoundFinished;
    }
	
}
