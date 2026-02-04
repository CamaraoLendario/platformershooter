using Godot;
using System;
using System.Collections.Generic;

public partial class Controller : Node
{
	[Export] public AnimatedSprite2D sprite;
	[Export] public CollisionShape2D collision;
	[Export] public PlayerInput playerInput;
	public Vector2 inputVector = Vector2.Zero;
	public bool isPilotController = false;
	protected float controllerDeadzone = 0.3f;
	public Player Main;
	public Vector2 Velocity
	{
		get
		{
			return Main.Velocity;
		}
		set
		{
			Main.Velocity = value;
		}
	}
	public Vector2 Position
	{
		get
		{
			return Main.Position;
		}
		set
		{
			Main.Position = value;
		}
	}
	public List<Timer> timers = [];

	public override void _Ready()
	{
		Main = GetParent<Player>();

		playerInput.InputDirChanged += ProcessWASD;
		playerInput.JumpStart += OnJumpStart;
		playerInput.JumpEnd += OnJumpEnd;
		playerInput.AimStart += OnAimStart;
		playerInput.AimEnd += OnAimEnd;
		playerInput.ShootStart += OnShootStart;
		playerInput.ShootEnd += OnShootEnd;
		playerInput.MeleeStart += OnMeleeStart;
		playerInput.MeleeEnd += OnMeleeEnd;
		playerInput.DropStart += OnDropStart;
		playerInput.DropEnd += OnDropEnd;
	}
	public void SetupTimers(List<Timer> timers)
	{
		foreach (Timer timer in timers)
		{
			SetupTimer(timer);
		}
	}
	public void SetupTimer(Timer timer)
	{
		timer.OneShot = true;
		CallDeferred(MethodName.AddChild, timer);
	} 

	public virtual void Start()
	{
		if (Main.IsDead) return;
		sprite.Visible = true;
		collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
	}
	public virtual void End()
	{
		sprite.Visible = false;
		collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

 	/// <summary>
	/// Called when input for movement direction changes (WASD, joystick, etc).
	/// </summary>
	public virtual void ProcessWASD(float X, float Y)
	{
	}

	/// <summary>
	/// Called when "Jump" input begins.
	/// </summary>
	public virtual void OnJumpStart()
	{
	}

	/// <summary>
	/// Called when "Jump" input ends.
	/// </summary>
	public virtual void OnJumpEnd()
	{
	}

	/// <summary>
	/// Called when "Aim" input begins.
	/// </summary>
	public virtual void OnAimStart()
	{
	}

	/// <summary>
	/// Called when "Aim" input ends.
	/// </summary>
	public virtual void OnAimEnd()
	{
	}

	/// <summary>
	/// Called when "Shoot" input begins.
	/// </summary>
	public virtual void OnShootStart()
	{
	}

	/// <summary>
	/// Called when "Shoot" input ends.
	/// </summary>
	public virtual void OnShootEnd()
	{
	}

	/// <summary>
	/// Called when "Melee" input begins.
	/// </summary>
	public virtual void OnMeleeStart()
	{
	}

	/// <summary>
	/// Called when "Melee" input ends.
	/// </summary>
	public virtual void OnMeleeEnd()
	{
	}
	
	/// <summary>
	/// Called when "Drop" input begins.
	/// </summary>
	public virtual void OnDropStart()
	{
	}

	/// <summary>
	/// Called when "Drop" input ends.
	/// </summary>
	public virtual void OnDropEnd()
	{
	}

	public bool IsAllowed()
	{
		if (Main.isPilot != isPilotController || Main.IsDead)
			return false;
		return true;
	}

	public virtual void Reset()
	{
		inputVector *= 0;
		Main.isAiming = false;
		foreach(Timer timer in timers)
		{
			timer.Stop();
		}
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		
		playerInput.InputDirChanged -= ProcessWASD;
		playerInput.JumpStart -= OnJumpStart;
		playerInput.JumpEnd -= OnJumpEnd;
		playerInput.AimStart -= OnAimStart;
		playerInput.AimEnd -= OnAimEnd;
		playerInput.ShootStart -= OnShootStart;
		playerInput.ShootEnd -= OnShootEnd;
		playerInput.MeleeStart -= OnMeleeStart;
		playerInput.MeleeEnd -= OnMeleeEnd;
		playerInput.DropStart -= OnDropStart;
		playerInput.DropEnd -= OnDropEnd;
	}
}