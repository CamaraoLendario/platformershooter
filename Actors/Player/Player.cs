
using Godot;
using System.Collections.Generic;
using static SpaceMages.SpaceMagesVars;
using System.Security.AccessControl;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System;

public partial class Player : CharacterBody2D
{
	#region Signals
	[Signal] public delegate void TookDamageEventHandler(Player player);
	[Signal] public delegate void DiedEventHandler(Player player, Player killer);
	[Signal] public delegate void ResetingEventHandler();
	[Signal] public delegate void WentPilotAreaEventHandler();
	[Signal] public delegate void WentShipAreaEventHandler();
	[Signal] public delegate void LostShipEventHandler();
	#endregion
	[Export] public bool godMode = false;
	public bool hasShield = true;
	#region Nodes
	[ExportGroup("Nodes")]
	[Export] public PlayerEffectHandler effectHandler;
	[Export] public PilotAttack pilot;
	[Export] public ShipAttack ship;
	[Export] public PlayerInput inputComponent;
	[Export] public Label NameLabel;
	[Export] PilotWeaponHolder pilotWeaponHolder;
	[Export] MeleeAttack pilotMeleeAttack;
	[Export] AudioStreamPlayer2D shipDeadAudio;
	[Export] AudioStreamPlayer pilotDeadAudio;
	[Export] public AnimatedSprite2D pilotSprite;
	[Export] public AnimatedSprite2D shipSprite;
	[Export] public AnimatedSprite2D shipShield;
	[Export] public AnimatedSprite2D pilotShield;
	[Export] public AnimationPlayer pilotShieldFlickerer;
	[Export] public AnimationPlayer shipShieldFlickerer;
	[Export] public ParticlesHandler particlesHandler;
	public const string playerSceneUID = "uid://cbmq3xh2bcijs";
	#endregion
	public Controller currentController;
	public int colorIdx = -1; // this will eventually be class instead
	public World world;
	
	#region Timers
	Timer shipCooldown = new();
	public const float SHIPCOOLDOWNTIME = 5.0f;
	Timer goShipTimer = new();
	public const float TIMETOSHIP = 2.0f;
	public Timer shieldCooldownTimer = new();
	const float SHIELDCOOLDOWNTIME = 15f;
	Timer shipPardonTimer = new();
	const float SHIPPARDONTIME = 0.2f;
	const float NAMEHIDETIME = 2f;
	List<Timer> timers = [];
	#endregion
	bool isInvulnerable = false;
	const int IFRAMES = 3;

	public bool isDead = false;

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
				EmitSignal(SignalName.WentPilotArea);
			}
			else
			{
				GoShip();
				EmitSignal(SignalName.WentShipArea);
			}
		}
	}
	private bool isInPilotArea = false;
	public bool isPilot = false;
	public List<CollisionShape2D> collisionShapes = [];
	public bool isAiming = false;

	public override void _Ready()
	{
		currentController = ship;
		SetupTimersVarsAndSignals();
		pilotShieldFlickerer.CurrentAnimation = "shieldRegeneration";
		shipShieldFlickerer.CurrentAnimation = "shieldRegeneration";
		MoveAndSlide();
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
		(Material as ShaderMaterial).SetShaderParameter("Color", teamColors[colorIdx]);
		particlesHandler.SetColor(colorIdx);
		(shipShield.Material as ShaderMaterial).SetShaderParameter("Color", teamColors[colorIdx]);
		(pilotShield.Material as ShaderMaterial).SetShaderParameter("Color", teamColors[colorIdx]);

		this.colorIdx = colorIdx;
	}
	public bool TakeDamage(Player damageDealer = null)
	{
		//TODO somewhere somehow you can take damage at the start of the round wtf is going on
		if (isDead || isInvulnerable || godMode) return false;
		if (damageDealer == null) damageDealer = this;
		
		if (!isPilot) //isShip
		{
			Input.StartJoyVibration(GetInputIdx(), 0.6f, 0.6f, 0.4f);
			Shake(0.5f, 5);
			shipCooldown.Start(SHIPCOOLDOWNTIME);
			GoPilot();
			shipDeadAudio.PitchScale = 1 + (float) GD.RandRange(-0.1, 0.1);
			shipDeadAudio.Play();
			EmitSignal(SignalName.LostShip);
		}
		else if (hasShield)
		{
			Input.StartJoyVibration(GetInputIdx(), 0.3f, 0.3f, 0.2f);
			Shake(0.5f, 3);
			BreakShield();
		}
		else
		{
			Die(damageDealer);
		}
		
		HandleIFrames();
		EmitSignal(SignalName.TookDamage, damageDealer);
		GD.Print(this.Name, " took Damage from by ", damageDealer.Name);
		return true;
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

	public void GoPilot()
	{	
		if(isDead) return;
		pilot.Start();
		ship.End();
		if (currentController is not PlayerDebugComponent) currentController = pilot;	

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

	public void TryGoShip()
	{
		TryGoShip(false);
	}
	public void TryGoShip(bool forced)
	{
		//GD.Print("trying to go ship...");
		if ((goShipTimer.IsStopped() && shipCooldown.IsStopped() && !isInPilotArea) || !shipPardonTimer.IsStopped() || forced)
		{
			//GD.Print("Succeded!");
			if (Velocity.LengthSquared() > 0.1)
				shipSprite.Rotation = Velocity.Angle();
			shipPardonTimer.Stop();
			isPilot = false;
			ship.Start();
			pilot.End();
			if (currentController is not PlayerDebugComponent)currentController = ship;
		}
		else GD.Print("try go ship failed..");
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
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Quint);
		tween.SetEase(Tween.EaseType.In);
		isDead = false;
		NameLabel.Modulate = new Color(1, 1, 1, 1);
		tween.TweenProperty(NameLabel, "modulate:a", 0, NAMEHIDETIME);
		
		Velocity *= 0;
		pilot.Reset();
		ship.Reset();
		RecoverShield();
		pilotShieldFlickerer.Stop();
		shipShieldFlickerer.Stop();
		goShipTimer.Stop();
		shipPardonTimer.Stop();
		foreach (Timer timer in timers)
		{
			timer.Stop();
		}

		if (Game.GetMap().IsPositionInPilotArea(Position))
		{
			GoPilot();
		}
		else TryGoShip(true);
		Show();
		if (Game.GetMap().IsPositionInPilotArea(Position))
			GoPilot();
		else
			TryGoShip(true);
		CallDeferred(MethodName.EmitSignal, SignalName.Reseting);
	}

	void SetupTimersVarsAndSignals()
	{
		inputComponent.AimStart += OnAimStart;
		inputComponent.AimEnd += OnAimEnd;
		Game.Instance.UnPausedGame += () =>
		{
			isAiming = Input.IsActionJustPressed("Aim" + inputComponent.keyboardKeyword + GetInputIdx());	
		};
		
		//GD.Print("MY PLAYER COLOR INDEX IS THIS: " + colorIdx);

		world = GetTree().GetFirstNodeInGroup("World") as World;

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
		pilotWeaponHolder.WeaponShot += shipPardonTimer.Stop;
		pilotMeleeAttack.meleed += shipPardonTimer.Stop;
		AddChild(shieldCooldownTimer);
		shieldCooldownTimer.Timeout  += () =>
		{
			pilotShieldFlickerer.Play("shieldRegeneration");
			shipShieldFlickerer.Play("shieldRegeneration");
		};
		timers = [shipCooldown, goShipTimer, shieldCooldownTimer];
	}
	public void SetInputIdx(int newInputIdx)
	{
		inputComponent.inputIdx = newInputIdx;
	}
	public int GetInputIdx()
	{
		return inputComponent.inputIdx;
	}
	public void SetPlayerName(string newName)
	{
		Name = $"Player: {newName}";
		NameLabel.Text = newName;
	}
	public static Player New(PlayerInfo playerInfo)
	{
		return New(playerInfo.Name, playerInfo.colorIdx, playerInfo.inputIdx);
	}
	public static Player New(string newName, int colorIdx, int inputIdx)
	{
		Player player = GD.Load<PackedScene>(playerSceneUID).Instantiate<Player>();

		player.SetPlayerName(newName);
		player.SetColor(colorIdx);
		player.SetInputIdx(inputIdx);

		return player;
	}
	public int GetColorIdx()
	{
		return colorIdx;
	}
	void Die(Player killer)
	{
		Input.StartJoyVibration(GetInputIdx(), 0.8f, 0.8f, 0.6f);
		Velocity *= 0;
		isDead = true;

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


		particlesHandler.CreateDeathParticles(killer);
		Position = new Vector2(99999, 99999);
		EmitSignal(SignalName.Died, this, killer);
		SignalBus.Instance.EmitSignal(SignalBus.SignalName.playerDied, this, killer);
		pilotDeadAudio.PitchScale = 1 + (float) GD.RandRange(-0.1, 0.1);
		pilotDeadAudio.Play();
		GD.Print(this, " was killed by ", killer);
	}

	void BreakShield()
	{
		pilotShield.SelfModulate = new Color(1, 1, 1, 0);
		shipShield.SelfModulate = new Color(1, 1, 1, 0);
		shieldCooldownTimer.Start(SHIELDCOOLDOWNTIME - 0.74f); //magic number is animation length for shield flickering. can't get it while its not playing
		particlesHandler.SummonShieldBreakParticles(isPilot);
		hasShield = false;
	}
	void RecoverShield()
	{
		pilotShield.SelfModulate = new Color(1, 1, 1, 0.75f);
		shipShield.SelfModulate = new Color(1, 1, 1, 0.75f);
		hasShield = true;
	}
	public AnimatedSprite2D GetCurrentSprite()
	{
		if (isPilot)
			return pilotSprite;
		else
			return shipSprite;
	}
	# if TOOLS
    public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey key && Input.IsKeyPressed(Key.L))
		{
			Die(this);
		}
	}
	# endif
}
