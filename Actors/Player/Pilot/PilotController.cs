using Godot;
using System.Collections.Generic;
public partial class PilotController : Controller
{
	[Signal] public delegate void JumpedEventHandler(int wallSide);
	[Export] RayCast2D rightWallCheck;
	[Export] RayCast2D leftWallCheck;
	[Export] AudioStreamPlayer2D jumpAudioPlayer;
	public Vector2 aimVector = new Vector2(1, 0);
	public int facing = 1;
	protected bool isDashing = false;
	protected bool isJumping = false;
	#region Timers
	Timer jumpBufferTimer  = new Timer();
	const float JUMPBUFFERTIME  = 0.1f;
	Timer coyoteTimer = new Timer();
	const float COYOTETIME = 0.1f;
	Timer wallCoyoteTimer = new Timer();
	public const float MAXHSPEED = 500f;
	#endregion
	public const float SOFTMAXHSPEED = 150f;
	public float acceleration = 3000f;
	public float groundFrictionIndex = 1500f;
	const float AIRFRICTIONINDEX = 1000f;
	public const float JUMPFORCE = 500f;
	const float GRAVITYFORCE = 1500f;
	Vector2 gravityDirection = new Vector2(0, 1);
	const float MAXFALLSPEED = 600f;
	bool hasAirJump = true;
	bool isFrozen
	{
		get
		{
			return Main.effectHandler.isFrozen;
		}
		set
		{
			Main.effectHandler.isFrozen = value;
		}
	}

	public override void _Ready()
	{
		base._Ready();
		isPilotController = true;
		List <Timer> pilotTimers = [coyoteTimer, wallCoyoteTimer, jumpBufferTimer];
		foreach(Timer timer in pilotTimers)
		{
			timers.Add(timer);
		}
		SetupTimers([coyoteTimer, wallCoyoteTimer, jumpBufferTimer]);
	}
    public override void _PhysicsProcess(double delta)
    {
        SetAimAndFacing();
    }

	public override void ProcessPhysics(double delta)
	{
		if (!IsAllowed()) return;
		
		ProcessMovement((float)delta);

		bool wasOnFloor = Main.IsOnFloor();
		bool wasOnWall = Main.IsOnWall();
		Main.MoveAndSlide();

		if (Velocity.Y > 0) isJumping = false;

		if (!Main.IsOnFloor() && wasOnFloor)
			coyoteTimer.Start(COYOTETIME);

		if (!Main.IsOnWall() && wasOnWall)
			wallCoyoteTimer.Start(COYOTETIME);
	}
	public override void ProcessWASD(float X, float Y)
	{
		if (Main.effectHandler.isFrozen) return;
		inputVector = new Vector2(X, Y);		
	}

	void SetAimAndFacing()
	{
		Vector2 processedInput = GetProcessedInput().Normalized();
		if ((!Main.IsInPilotArea || Main.isAiming) && processedInput != Vector2.Zero)
		{
			aimVector = processedInput;
		}
		else if (!Main.IsInPilotArea )
		{
			aimVector = new Vector2(Mathf.Cos(sprite.Rotation - Mathf.Pi/2), Mathf.Sin(sprite.Rotation - Mathf.Pi/2));
		}
		else if (processedInput.Y > 0.5 || processedInput.Y < -0.5)
		{
			aimVector = new Vector2(0, processedInput.Y).Normalized();
		}
		else aimVector = new Vector2(facing, 0);


		if (Main.IsOnWall() && !Main.IsOnFloor())
			facing = -GetWallSide();
		else if (processedInput.X != 0)
		{
			facing = (int)new Vector2(processedInput.X, 0).Normalized().X;
		}
	}
	
	void ProcessMovement(float delta)
	{
		if (!Main.IsInPilotArea)
		{
			SpaceMove((float) delta);
		}
		else if (Main.IsOnFloor())
		{
			Move(acceleration, groundFrictionIndex, delta);
			hasAirJump = true;
		}
		else
		{
			Move(acceleration/2, AIRFRICTIONINDEX, delta);
			ProcessGravity(delta);
		}
	}
	void Move(float acceleration, float friction, float delta)
	{
		Vector2 newVel = Velocity;
		Vector2 processedInputVector = GetProcessedInput();
		if (isFrozen && IsOnFloor())
		{
			friction /= 5;
		}
		int XinputDirection = (int)new Vector2(processedInputVector.X, 0).Normalized().X;
		if (processedInputVector != Vector2.Zero)
		{
			processedInputVector.X += XinputDirection * 0.4f;
			if (processedInputVector.X > 1.0 || processedInputVector.X < -1.0)
				processedInputVector.X = XinputDirection;
		}

		if (!(Velocity.X * Velocity.X > SOFTMAXHSPEED * SOFTMAXHSPEED && Velocity.X * processedInputVector.X > 0) && !Main.isAiming)
		{
			newVel.X += processedInputVector.X * acceleration * delta;
		}

		if (newVel.X != 0 && !(isDashing && !Main.IsOnFloor()))
		{
			newVel.X -= new Vector2(newVel.X, 0).Normalized().X * friction * delta;
		}

		if (newVel.X * Velocity.X < 0)
		{
			newVel.X = 0;
		}
		else if (Velocity.X * Velocity.X > MAXHSPEED * MAXHSPEED && !Main.IsOnWall())
		{
			newVel = new Vector2(MAXHSPEED * new Vector2(newVel.X, 0).Normalized().X, newVel.Y);
		}

		Velocity = newVel;
	}
	void SpaceMove(float delta)
	{
		if (!Main.isAiming) Velocity += GetProcessedInput().Normalized() * acceleration * delta;

		float dragMag = AIRFRICTIONINDEX * 0.05f;
 
		Velocity -= Velocity.Normalized() * dragMag * delta;

		if (Velocity.LengthSquared() > SOFTMAXHSPEED * SOFTMAXHSPEED)
			Velocity = Velocity.Normalized() * SOFTMAXHSPEED;
		if (Velocity.LengthSquared() < 0.1)
			Velocity *= 0;
	}

	public Vector2 GetProcessedInput()
	{
		Vector2 processedInputVector = inputVector;

		if (processedInputVector.Length() < controllerDeadzone)
		{
			processedInputVector *= 0;
		}
	
		return processedInputVector;
	}
	
	void ProcessGravity(float delta)
	{
		if (!Main.IsInPilotArea || isDashing) return;
		if(Velocity.Y == MAXFALLSPEED && !Main.IsOnWall()) return;
		float tempMaxFallSpeed = MAXFALLSPEED;

		if (Main.IsOnWall() && Velocity.Y > 0)
		{
			tempMaxFallSpeed *= 0.1f;
		}

		Velocity += gravityDirection * GRAVITYFORCE * delta;
		if(Velocity.Y > tempMaxFallSpeed)
		{
			Velocity = new Vector2(Velocity.X, tempMaxFallSpeed);
		}
	}

	public override void OnJumpStart()
	{
		if(!IsAllowed() || !Main.IsInPilotArea) return;
		if (Main.effectHandler.isFrozen) return;
		ProcessJump();
	}
	public override void OnJumpEnd()
	{
		if(!IsAllowed()) return;
		if (!isJumping) return;
		isJumping = false;
		Vector2 newVel = Velocity;
		newVel.Y *= 0.35f;
		Velocity = newVel;
	}

	void ProcessJump(bool isBufferInput = false)
	{
		if ((!wallCoyoteTimer.IsStopped() || !coyoteTimer.IsStopped()) && isDashing)
		{
			wallCoyoteTimer.Stop();
			coyoteTimer.Stop();
		}
		if (!isBufferInput) jumpBufferTimer.Stop();

		if ((Main.IsOnFloor() || !coyoteTimer.IsStopped() || hasAirJump && !Main.IsOnWall()) && wallCoyoteTimer.IsStopped())
		{
			Jump();
			if (!Main.IsOnFloor())
				hasAirJump = !coyoteTimer.IsStopped();
		}
		else if (!wallCoyoteTimer.IsStopped() || Main.IsOnWall())
		{
			wallJump();
			hasAirJump = true;
		}
		else if (jumpBufferTimer.IsStopped())
		{
			jumpBufferTimer.Start(JUMPBUFFERTIME);
		}
	}

	void Jump()
	{
		isJumping = true;
		jumpAudioPlayer.PitchScale = 1 + (float)GD.RandRange(-0.1f, 0.1f);
		jumpAudioPlayer.Play();
		Vector2 newVel = Velocity;
		newVel.Y = -JUMPFORCE;
		Velocity = newVel;
		EmitSignal(SignalName.Jumped, GetWallSide());
	}
	void wallJump()
	{
		isJumping = true;
		jumpAudioPlayer.PitchScale = 1 + (float)GD.RandRange(-0.1f, 0.1f);
		jumpAudioPlayer.Play();
		int wallSide = GetWallSide();
		Velocity = new Vector2(-wallSide * 1, -1).Normalized() * JUMPFORCE;
		EmitSignal(SignalName.Jumped, wallSide);
	}

	public int GetWallSide()
	{
		if (rightWallCheck.IsColliding()) { return 1; }
		if (leftWallCheck.IsColliding()) { return -1; }
		return 0;
	}

	public override void Start()
	{
		base.Start();
		hasAirJump = true;
	}

	bool IsOnFloor()
	{
		return Main.IsOnFloor();
	}

    public override void Reset()
    {
        base.Reset();
		sprite.Rotation = 0;
    }

}
