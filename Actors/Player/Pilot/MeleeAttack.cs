using Godot;
using System;
using System.Threading.Tasks;

public partial class MeleeAttack : Area2D
{
	[Signal] public delegate void meleedEventHandler();
	[Signal] public delegate void dashedEventHandler();
	[Signal] public delegate void dashEndEventHandler();
	[Export] AudioStreamPlayer2D meleeAudioPlayer;
	[Export] AnimatedSprite2D sprite;
	[Export] CollisionShape2D collisionShape;
	[Export] PlayerParticleEmitters dashParticles;
	[Export] Player Main;
	[Export] float dashForce = 500f;
	Timer cooldownTimer = new();
	const float COOLDOWNTIME = 0.35f;
	Timer dashTimer = new();
	const float DASHTIME = 0.025f;
	int meleeActiveFrames = 8;
	bool isMeleeing = false;
	int colorIdx;
	
	PilotAttack pilot;
	bool hasDeflectionPrivelage = false;

	public override void _Ready()
	{
		colorIdx = Main.colorIdx;
		pilot = Main.pilot;
		pilot.playerInput.InputDirChanged += SetDirection;
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
		
		cooldownTimer.OneShot = true;
		AddChild(cooldownTimer);
		dashTimer.OneShot = true;
		AddChild(dashTimer);
		dashTimer.Timeout += () =>
		{
			EmitSignal(SignalName.dashEnd);
		};
	}


	public void Attack()
	{
		if (!cooldownTimer.IsStopped() && !hasDeflectionPrivelage) return;

		EmitSignal(SignalName.meleed);
		meleeAudioPlayer.PitchScale = 1 + (float)GD.RandRange(-0.1f, 0.1f);
		meleeAudioPlayer.Play();
		hasDeflectionPrivelage = false;
		if (Main.IsOnFloor())
			cooldownTimer.Start(COOLDOWNTIME);
		else
			cooldownTimer.Start(COOLDOWNTIME * 2);

		TriggerDash();

		sprite.Frame = 0;
		sprite.Play();
		HandleHitbox();
	}

	async void HandleHitbox()
	{
		collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		isMeleeing = true;
		for (int i = 0; i < meleeActiveFrames; i++)
		{
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		}
		isMeleeing = false;
		collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	} 

	void TriggerDash()
	{
		if (Main.effectHandler.isFrozen) return;
		Vector2 attackDirNormal = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));
		if (!Main.IsOnFloor() && Main.IsInPilotArea) SummonParticles(-attackDirNormal);
		EmitSignal(SignalName.dashed);
		dashTimer.Start(DASHTIME);
		float tempDashForce = dashForce;
		if (Main.IsOnFloor()) tempDashForce *= 0.5f;

		pilot.Velocity = attackDirNormal * tempDashForce;
	}

	void SetDirection(float X, float Y)
	{
		if (isMeleeing)
        {
            return;
        }

		Vector2 inputVec = pilot.aimVector;
		if  (inputVec == Vector2.Zero){
			inputVec = new Vector2(pilot.facing, 0);
		}
		
		Rotation = inputVec.Angle();

		if (Mathf.Cos(Rotation) > 0)
			sprite.FlipV = false;
		else sprite.FlipV = true;
		if (sprite.FlipV)
			sprite.Offset = Vector2.Up * 4;
		else sprite.Offset = Vector2.Down * 4;
	}


	void OnAreaEntered(Node2D area)
	{
		if (area is LinearProjectile projectile/*  && projectile is not PoisonProjectile */)
		{
			projectile.Direction = pilot.aimVector.Normalized();
			projectile.owner = Main;
			//projectile.End();
			hasDeflectionPrivelage = true;
		}
		
		if (area is MeleeAttack melee)
		{
		   if (!Main.IsOnFloor() || !(MathF.Sin(Rotation) < 0.708f && MathF.Sin(Rotation) > -0.708f)) pilot.Velocity = (GlobalPosition - melee.GlobalPosition).Normalized() * PilotController.JUMPFORCE;
		   else pilot.Velocity = new Vector2(-pilot.facing, -1).Normalized() * PilotController.JUMPFORCE;
		}
		
		if (area is DestructibleBlockFlag destructibleBlockFlag)
			destructibleBlockFlag.Destroy();
        
	}
	void OnBodyEntered(Node2D body)
	{
		foreach (Area2D area in GetOverlappingAreas())
		{
			if (area is MeleeAttack) return;
		}

		if (body is Player player)
		{
			if (player.isPilot && player.colorIdx != colorIdx)
			{
				(RayCast2D ray, bool hasLOS) LOS = getHasLOS(player); 
				if (LOS.hasLOS)
					player.TakeDamage(Main);
				LOS.ray.QueueFree();
			}
		}
	}
	void SummonParticles(Vector2 direction)
	{
		PlayerParticleEmitters newDashParticles = dashParticles.Duplicate() as PlayerParticleEmitters;
		((newDashParticles.GetChild(0) as GpuParticles2D).ProcessMaterial as ParticleProcessMaterial).Direction = new Vector3(direction.X, direction.Y, 0);

		newDashParticles.GlobalPosition = Main.GlobalPosition + Vector2.Up;
		Game.Instance.world.AddChild(newDashParticles);
		newDashParticles.Emit();
	}
	(RayCast2D ray, bool hasLOS) getHasLOS(Player player)
	{
		RayCast2D checkRay = new RayCast2D();
		checkRay.Position = GlobalPosition;
		checkRay.TargetPosition = player.Position - GlobalPosition;
		checkRay.SetCollisionMaskValue(2, true);
		checkRay.HitFromInside = true;
		checkRay.AddException(Main);
		Game.Instance.world.AddChild(checkRay);
		return (checkRay, helpLOS(checkRay, player));
	}	
	bool helpLOS(RayCast2D checkRay, Player player)
	{      
		checkRay.ForceRaycastUpdate();
		
		if (!checkRay.IsColliding()) return false;

	   /*  if (checkRay.GetCollider() is TileMapLayer tileMapLayer && checkRay.GetCollisionNormal() == Vector2.Zero)
		{
			checkRay.AddException(tileMapLayer.shape);
			return helpLOS(checkRay, player);
		}
		else */ if (checkRay.GetCollider() == player)
			return true;
		else
			return false;
	}

	public int GetIDX()
	{
		return Main.inputIdx;
	}
}
