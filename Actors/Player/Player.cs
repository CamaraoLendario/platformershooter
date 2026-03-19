using Godot;
using System.Collections.Generic;
using SpaceMages;
using System.Security.AccessControl;
using System.IO.Pipes;
using System.Runtime.CompilerServices;

public partial class Player : CharacterBody2D
{
	#region Signals
	[Signal] public delegate void tookDamageEventHandler(Player player);
	[Signal] public delegate void diedEventHandler(Player player, Player killer);
	[Signal] public delegate void resetingEventHandler();
	#endregion
	[Export] public bool godMode = false;
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
				pilotShield.SelfModulate = new Color(1, 1, 1, 0.75f);
				shipShield.SelfModulate = new Color(1, 1, 1, 0.75f);
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
	[Export] GpuParticles2D shipExplosionParticles;
	[Export] GpuParticles2D shipReconstructionParticles;
	GpuParticles2D currentShipReconstructionParticles;
	[Export] GpuParticles2D pilotDeathParticles;
	[Export] AudioStreamPlayer2D shipDeadAudio;
	[Export] AudioStreamPlayer pilotDeadAudio;
	[Export] AnimatedSprite2D pilotSprite;
	[Export] AnimatedSprite2D shipSprite;
	[Export] public AnimatedSprite2D shipShield;
	[Export] public AnimatedSprite2D pilotShield;
	[Export] public AnimationPlayer pilotShieldFlickerer;
	[Export] public AnimationPlayer shipShieldFlickerer;
	#endregion
	public Controller currentController;
	public int inputIdx = -2;
	public int colorIdx = -1;
	public bool isKeyboardControlled = false;
	World world;
	
	#region Timers
	Timer shipCooldown = new();
	const float SHIPCOOLDOWNTIME = 5.0f;
	Timer goShipTimer = new();
	const float TIMETOSHIP = 2.0f;
	public Timer shieldCooldownTimer = new();
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
				foreach(CollisionShape2D pilotColShape in GetTree().GetNodesInGroup("PilotCollisions"))
				{
					if (pilotColShape.GetParent<Player>() == this)
						pilotColShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
				}
				foreach(CollisionShape2D shipColShape in GetTree().GetNodesInGroup("ShipCollisions"))
				{
					if (shipColShape.GetParent<Player>() == this)
						shipColShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
				}
				//pilot.collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
				//ship.collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
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
				CancelTurningShip();
			}
			else
			{
				GoShip();
			}
		}
	}
	private bool isInPilotArea = false;
	public bool isPilot = false;
	bool isTurningToShip = false;
	public List<CollisionShape2D> collisionShapes = [];
	public bool isAiming = false;

	public override void _Ready()
	{
		currentController = ship;
		SetupTimersVarsAndSignals();
		pilotShieldFlickerer.CurrentAnimation = "shieldRegeneration";
		shipShieldFlickerer.CurrentAnimation = "shieldRegeneration";
	}
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		currentController.ProcessPhysics(delta);
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
		(pilotDeathParticles.ProcessMaterial as ShaderMaterial).SetShaderParameter("outlineColor", SpaceMagesVars.teamColors[colorIdx]);
		
		(shipShield.Material as ShaderMaterial).SetShaderParameter("Color", SpaceMagesVars.teamColors[colorIdx]);
		(pilotShield.Material as ShaderMaterial).SetShaderParameter("Color", SpaceMagesVars.teamColors[colorIdx]);

		this.colorIdx = colorIdx;
	}

	public void TakeDamage(int ID)
	{
		TakeDamage(Game.Instance.playerNodesByColor[ID]);
	}
	public void TakeDamage(Player damageDealer = null)
	{
		if (isDead || isInvulnerable || godMode) return;
		if (damageDealer == null) damageDealer = this;

		if (HasShield)
		{
			Input.StartJoyVibration(inputIdx, 0.3f, 0.3f, 0.2f);
			Shake(0.5f, 3);
			HasShield = false;
		}
		else if (isPilot)
		{
			Input.StartJoyVibration(inputIdx, 0.8f, 0.8f, 0.6f);
			IsDead = true;
			//CreateRagdol(damageDealer);
			CreateDeathParticles(damageDealer);
			Position = new Vector2(99999, 99999);
			EmitSignal(SignalName.died, this, damageDealer);
			pilotDeadAudio.PitchScale = 1 + (float) GD.RandRange(-0.1, 0.1);
			pilotDeadAudio.Play();
			GD.Print(this, " was killed by ", damageDealer);
		}
		else
		{
			GpuParticles2D newShipExplosionParticles = GPUParticlesPool.GetClonedParticles(shipExplosionParticles);
			newShipExplosionParticles.Position = Position;
			newShipExplosionParticles.Emitting = true;
			Input.StartJoyVibration(inputIdx, 0.6f, 0.6f, 0.4f);
			Shake(0.5f, 5);
			shipCooldown.Start(SHIPCOOLDOWNTIME);
			GoPilot();
			shipDeadAudio.PitchScale = 1 + (float) GD.RandRange(-0.1, 0.1);
			shipDeadAudio.Play();
		}
		HandleIFrames();
		EmitSignal(SignalName.tookDamage, damageDealer);
		GD.Print(this.Name, " took Damage from by ", damageDealer.Name);
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
		
		//GpuParticles2D newParticlesEmitter = particlesEmitter.Duplicate() as GpuParticles2D;
		GpuParticles2D newParticlesEmitter = GPUParticlesPool.GetClonedParticles(particlesEmitter);
		(newParticlesEmitter.Material as ShaderMaterial).SetShaderParameter("Color", SpaceMagesVars.teamColors[colorIdx]);
		newParticlesEmitter.Position = Position;
		newParticlesEmitter.OneShot = true;
		thisissofuckingweirdwhatdemonhaspocessedthisgameatleastitworksIguessbutatwhatcost(newParticlesEmitter);
	}
	async void thisissofuckingweirdwhatdemonhaspocessedthisgameatleastitworksIguessbutatwhatcost(GpuParticles2D newParticlesEmitter)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		newParticlesEmitter.Restart();
	}
	public void ForceRecoverShield()
	{
		HasShield = true;
	}

	public void GoPilot()
	{	
		if(isDead) return;
		pilot.Start();
		ship.End();
		if (currentController is not PlayerDebugComponent) currentController = pilot;	

		if (!isPilot) shipPardonTimer.Start(SHIPPARDONTIME - shipReconstructionParticles.Lifetime);
		isPilot = true;
	}
	void GoShip()
	{	
		if(isDead) return;
		if (!shipPardonTimer.IsStopped())
		{
			TryGoShip();
		}
		else goShipTimer.Start(TIMETOSHIP - shipReconstructionParticles.Lifetime);
	}

	void PlayParticlesForTryGoShip()
	{
		GD.Print("Playing Particles For Ship Reconstruction...");
		if (isTurningToShip) return;
		GpuParticles2D particles = GPUParticlesPool.GetClonedParticles(shipReconstructionParticles);
		currentShipReconstructionParticles = particles;

		particles.Position = Vector2.Zero;
		particles.Restart();
		particles.Visible = true;
		isTurningToShip = true;
		//particles.ProcessMaterial = particles.ProcessMaterial.Duplicate() as ShaderMaterial;
		(particles.ProcessMaterial as ShaderMaterial).
		SetShaderParameter("outlineColor", SpaceMagesVars.teamColors[colorIdx]);
		(particles.ProcessMaterial as ShaderMaterial).
		SetShaderParameter("initPos", Position);
		(particles.ProcessMaterial as ShaderMaterial).
		SetShaderParameter("newPos", Position);
		particles.Finished += OnShipRebuiltFinished;
		(particles.ProcessMaterial as ShaderMaterial).
		SetShaderParameter("rotation", shipSprite.Rotation);
		
	}

    public override void _Process(double delta)
	{
		if (currentShipReconstructionParticles != null)
		{
			(currentShipReconstructionParticles.ProcessMaterial as ShaderMaterial).
			SetShaderParameter("newPos", Position);
		}
	}

	void OnShipRebuiltFinished()
	{
		GD.Print("rebuildFinished");
		isTurningToShip = false;
		RemoveParticles(currentShipReconstructionParticles);
		TryGoShip();
	}

	void RemoveParticles(GpuParticles2D particles)
	{
		if (currentShipReconstructionParticles == null) return;
		currentShipReconstructionParticles = null;
		particles.Finished -= OnShipRebuiltFinished;
		particles.Visible = false;
		particles.Emitting = false;
		GPUParticlesPool.Return(particles);
	}

	void CancelTurningShip()
	{
		if (!isTurningToShip) return;
		goShipTimer.Stop();
		shipPardonTimer.Stop();
		isTurningToShip = false;
		RemoveParticles(currentShipReconstructionParticles);
	}

	public void TryGoShip()
	{
		TryGoShip(false);
	}
	public void TryGoShip(bool forced)
	{
		GD.Print("trying to go ship...");
		if ((goShipTimer.IsStopped() && shipCooldown.IsStopped() && !isInPilotArea) || !shipPardonTimer.IsStopped() || forced)
		{
			GD.Print("Succeded!");
			shipPardonTimer.Stop();
			isPilot = false;
			ship.Start();
			pilot.End();
			if (currentController is not PlayerDebugComponent)currentController = ship;
		}
		else GD.Print("try go ship failed..");
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
	void CreateDeathParticles(Player damager)
	{
		GpuParticles2D particles = GPUParticlesPool.GetClonedParticles(pilotDeathParticles);
		(particles.ProcessMaterial as ShaderMaterial).SetShaderParameter("initialDir", (Position - damager.Position).Normalized());
		(particles.ProcessMaterial as ShaderMaterial).SetShaderParameter("rotation", pilotSprite.Rotation);
		particles.Position = Position;
		particles.Restart();
	}

	async void Shake(float shakeTime, float shakeForce = 1)
	{
		Timer shakeTimer = new Timer()
		{
			OneShot = true,
		};
		AddChild(shakeTimer);
		shakeTimer.Start(shakeTime);
		while(!shakeTimer.IsStopped())
		{
			float stepModifier = (float)shakeTimer.TimeLeft/shakeTime;
			float angle = GD.Randf() * Mathf.Pi * 2;
			Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			pilotSprite.Offset = Vector2.Zero + dir * shakeForce * stepModifier;
			shipSprite.Offset = Vector2.Zero + dir * shakeForce * stepModifier;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
		pilotSprite.Offset = Vector2.Zero;
		shipSprite.Offset = Vector2.Zero;
	}

	public void Reset()
	{
		EmitSignal(SignalName.reseting);
		
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Quint);
		tween.SetEase(Tween.EaseType.In);
		NameLabel.Modulate = new Color(1, 1, 1, 1);
		tween.TweenProperty(NameLabel, "modulate:a", 0, NAMEHIDETIME);
		
		pilot.Reset();
		ship.Reset();
		HasShield = true;
		pilotShieldFlickerer.Stop();
		shipShieldFlickerer.Stop();
		goShipTimer.Stop();
		shipPardonTimer.Stop();
		RemoveParticles(currentShipReconstructionParticles);
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

	void SetupTimersVarsAndSignals()
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
		shipCooldown.Timeout += PlayParticlesForTryGoShip;
		goShipTimer.OneShot = true;
		AddChild(goShipTimer);
		goShipTimer.Timeout += PlayParticlesForTryGoShip;
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
	}
}
