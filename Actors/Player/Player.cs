using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using SpaceMages;
using GodotPlugins.Game;
using System.Collections.ObjectModel;

public partial class Player : CharacterBody2D
{
	#region Signals
	[Signal] public delegate void tookDamageEventHandler(Player player);
	[Signal] public delegate void diedEventHandler(Player player, Player killer);
	[Signal] public delegate void resetingEventHandler();
	#endregion
	[Export] public bool HasShield
	{
		get
		{
			return hasShield;
		}
		set
		{
			if (!IsNodeReady()) return;
			hasShield = value;
			if (!value)
			{
				pilotShield.SelfModulate = new Color(1, 1, 1, 0);
				shipShield.SelfModulate = new Color(1, 1, 1, 0);
				shieldCooldownTimer.Start(SHIELDCOOLDOWNTIME - 0.74f); //magic number is animation length for shield flickering. can't get it while its not playing
				SummonShieldBreakParticles();
			}
			else
			{
				pilotShield.SelfModulate = new Color(1, 1, 1, 1);
				shipShield.SelfModulate = new Color(1, 1, 1, 1);
			}
		}
	}
	bool hasShield = true;
	[Export] PackedScene pilotRagdol;
	#region Nodes
	[ExportGroup("Nodes")]
	[Export] public PlayerEffectHandler effectHandler;
	[Export] public PilotAttack pilot;
	[Export] public ShipAttack ship;
	[Export] public PlayerInput playerInput;
	[Export] public Label NameLabel;
	[Export] PilotWeaponHolder pilotWeaponHolder;
	[Export] MeleeAttack pilotMeleeAttack;
	[Export] AudioStreamPlayer2D shipDeadAudio;
	[Export] AudioStreamPlayer2D pilotDeadAudio;
	[Export] AnimatedSprite2D pilotShield;
	[Export] AnimatedSprite2D shipShield;
	[Export] AnimationPlayer pilotShieldFlickerer;
	[Export] AnimationPlayer shipShieldFlickerer;
	#endregion

	public int inputIdx = -2;
	public int colorIdx = -1;
	public bool isKeyboardControlled = false;
	World world;
	
	#region Timers
	Timer shipCooldown = new();
	const float SHIPCOOLDOWNTIME = 5.0f;
	Timer goShipTimer = new();
	const float TIMETOSHIP = 2.0f;
	Timer shieldCooldownTimer = new();
	const float SHIELDCOOLDOWNTIME = 15f;
	Timer shipPardonTimer = new();
	const float SHIPPARDONTIME = 0.2f;
	const float NAMEHIDETIME = 2f;
	List<Timer> timers = [];
	#endregion
	bool isInvulnerable = false;
	const int IFRAMES = 3;

	public bool IsDead
	{
		get
		{
			return isDead;
		}
		set
		{
			Velocity *= 0;
			isDead = value;
			if (value)
			{
				Hide();
				pilot.collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
				ship.collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
				pilot.inputVector *= 0;
				ship.inputVector *= 0;
			}
			else
			{
				Show();
				if (world.currentMap.IsPositionInPilotArea(Position))
				{
					GoPilot();
				}
				else
				{
					TryGoShip(true);
				}
			}
		}
	}
	private bool isDead = false;

	public bool IsInPilotArea
	{
		get
		{
			return isInPilotArea;
		}
		set
		{
			if (isInPilotArea == value) return;
			isInPilotArea = value;
			if (value)
			{
				GoPilot();
			}
			else
			{
				GoShip();
			}
		}
	}
	private bool isInPilotArea = false;
	public bool isPilot = false;
	public List<CollisionShape2D> collisionShapes = [];
	public bool isAiming = false;

	public override void _Ready()
	{
		playerInput.AimStart += OnAimStart;
		playerInput.AimEnd += OnAimEnd;
		
		GD.Print("MY PLAYER COLOR INDEX IS THIS: " + colorIdx);

		world = Game.Instance.world;

		foreach (Node node in GetChildren())
		{
			if (node is CollisionShape2D shape)
				collisionShapes.Add(shape);
		}

		shipCooldown.OneShot = true;
		AddChild(shipCooldown);
		shipCooldown.Timeout += TryGoShip;
		goShipTimer.OneShot = true;
		AddChild(goShipTimer);
		goShipTimer.Timeout += TryGoShip;
		shipCooldown.OneShot = true;
		AddChild(shipPardonTimer);
		shipPardonTimer.OneShot = true;
		pilotWeaponHolder.WeaponShot += () => shipPardonTimer.Stop();
		pilotMeleeAttack.meleed += () => shipPardonTimer.Stop();
		AddChild(shieldCooldownTimer);
		shieldCooldownTimer.Timeout  += () =>
		{
			pilotShieldFlickerer.Play("shieldRegeneration");
			shipShieldFlickerer.Play("shieldRegeneration");
		};
		timers = [shipCooldown, goShipTimer, shieldCooldownTimer];

		pilotShieldFlickerer.CurrentAnimation = "shieldRegeneration";
		shipShieldFlickerer.CurrentAnimation = "shieldRegeneration";
	}

	public void OnAimStart()
	{
		isAiming = true;
	}

	public void OnAimEnd()
	{
		isAiming = false;
	}

	public void SetColor(int colorIdx)
	{
		(Material as ShaderMaterial).SetShaderParameter("Color", SpaceMagesVars.teamColors[colorIdx]);
		this.colorIdx = colorIdx;
	}

	public void TakeDamage(int ID)
	{
		TakeDamage(Game.Instance.playerNodesByColor[ID]);
	}
	public void TakeDamage(Player damageDealer)
	{
		if (damageDealer == null) damageDealer = this;
		if (isDead || isInvulnerable) return;
		HandleIFrames();
		if (HasShield)
		{
			HasShield = false;
		}
		else if (isPilot)
		{
			IsDead = true;
			CreateRagdol(damageDealer);
			EmitSignal(SignalName.died, this, damageDealer);
			pilotDeadAudio.PitchScale = 1 + (float) GD.RandRange(-0.1, 0.1);
			pilotDeadAudio.Play();
		}
		else
		{
			EmitSignal(SignalName.tookDamage, this);
			shipCooldown.Start(SHIPCOOLDOWNTIME);
			GoPilot();
			shipDeadAudio.PitchScale = 1 + (float) GD.RandRange(-0.1, 0.1);
			shipDeadAudio.Play();
		}
		
		GD.Print(this.Name, " was killed by ", damageDealer.Name);
	}
	
	async void HandleIFrames()
	{
		isInvulnerable = true;
		for (int i = 0; i < IFRAMES; i++)
		{
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		}
		isInvulnerable = false;
	} 
	void SummonShieldBreakParticles()
	{
		GpuParticles2D particlesEmitter;
		if (isPilot)
		{
			particlesEmitter = pilotShield.GetChild<GpuParticles2D>(0);
		}
		else
		{
			particlesEmitter = shipShield.GetChild<GpuParticles2D>(0);
		}
		
		GpuParticles2D newParticlesEmitter = particlesEmitter.Duplicate() as GpuParticles2D;
		newParticlesEmitter.Position = Position;
		world.AddChild(newParticlesEmitter);
		newParticlesEmitter.Emitting = true;

		newParticlesEmitter.Finished += () =>
		{
			newParticlesEmitter.QueueFree();
		};
	}
	public void ForceRecoverShield()
	{
		HasShield = true;
	}

	void GoPilot()
	{	
		if(isDead) return;
		pilot.Start();
		ship.End();
		if (isInPilotArea) Velocity = Velocity.Normalized() * 300f;
		if (!isPilot) shipPardonTimer.Start(SHIPPARDONTIME);
		isPilot = true;
	}
	void GoShip()
	{	
		if(isDead) return;
		if (!shipPardonTimer.IsStopped())
		{
			TryGoShip();
		}
		else goShipTimer.Start(TIMETOSHIP);
	}
	void TryGoShip()
	{
		TryGoShip(false);
	}
	void TryGoShip(bool forced)
	{
		if ((goShipTimer.IsStopped() && shipCooldown.IsStopped() && !isInPilotArea) || !shipPardonTimer.IsStopped() || forced)
		{
			shipPardonTimer.Stop();
			isPilot = false;
			ship.Start();
			pilot.End();
		}
	}

	void CreateRagdol(Player damager)
	{
		PilotRagdol newRagdol = pilotRagdol.Instantiate<PilotRagdol>();

		newRagdol.GlobalPosition = GlobalPosition;
		newRagdol.LinearVelocity = (Position - damager.Position).Normalized() * 250;
		(newRagdol.Material as ShaderMaterial).SetShaderParameter("Color", SpaceMagesVars.teamColors[colorIdx]);
		if (effectHandler.isPoisoned) (newRagdol.Material as ShaderMaterial).SetShaderParameter("PoisonBuildup", 1);
		else (newRagdol.Material as ShaderMaterial).SetShaderParameter("PoisonBuildup", 0); // not sure if this works?
		if (!isInPilotArea)
		{
			newRagdol.LinearVelocity *= 2.5f;
			newRagdol.IsInPilotArea = false;
		}
		Game.Instance.world.CallDeferred(MethodName.AddChild, newRagdol);
	}

	public void Reset()
	{
		EmitSignal(SignalName.reseting);
		
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Quint);
		tween.SetEase(Tween.EaseType.In);
		tween.TweenProperty(NameLabel, "modulate:a", 0, NAMEHIDETIME);
		
		pilot.Reset();
		ship.Reset();
		HasShield = true;
		pilotShieldFlickerer.Stop();
		shipShieldFlickerer.Stop();
		foreach (Timer timer in timers)
		{
			timer.Stop();
		}

		if (world.currentMap.IsPositionInPilotArea(Position))
		{
			GoPilot();
		}
		else TryGoShip(true);
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		Game.Instance.playerNodesByColor.Remove(colorIdx);
	}
}
