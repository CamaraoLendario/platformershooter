using Godot;
using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class ShipAttack : ShipController
{
	[Signal] public delegate void ShotEventHandler();
	
	[Export] public PackedScene bullet;
	[Export] AudioStreamPlayer2D ShootAudio;
	[Export] ShipPowerUpsHolder powerUpsHolder;
	const int MAXAMMO = 3;
	int currentAmmo = 3;

	#region Timers
	Timer bulletReloadTimer = new();
	float bulletreloadTime = 0.5f;
	Timer shootCooldownTimer = new();
	float shootCooldownTime = 0.1f;
	#endregion

	public override void _Ready()
	{
		SetupTimer(bulletReloadTimer);
		SetupTimer(shootCooldownTimer);
		bulletReloadTimer.Timeout += Reload;
		base._Ready();
		List<Timer> shipTimers = [bulletReloadTimer, shootCooldownTimer];
		foreach(Timer timer in shipTimers)
		{
			timers.Add(timer);
		}
	}


    public override void OnMeleeStart()
	{
		if (!IsAllowed() || Main.effectHandler.isFrozen || !shootCooldownTimer.IsStopped() || currentAmmo <= 0) return;
		
		if (!powerUpsHolder.HasDifferentShootingMechanics())
		{
			LinearProjectile newBullet = GetNewBullet();
			world.AddChild(newBullet);
		}
		
		EmitSignal(SignalName.Shot); 
		shootCooldownTimer.Start(shootCooldownTime);
		ShootAudio.PitchScale = 1 + (float) GD.RandRange(-0.1, 0.1);
		ShootAudio.Play();

		currentAmmo--;

		if (bulletReloadTimer.IsStopped())
        {
			bulletReloadTimer.Start(bulletreloadTime);
        }
	}

	public bool AddPowerUp(ShipPowerUp powerUp, int PowerUpID)
	{
		return powerUpsHolder.AddPowerUp(powerUp, PowerUpID);
	}

	void Reload()
	{
		if (currentAmmo < MAXAMMO)
		{
			currentAmmo++;
			bulletReloadTimer.Start(bulletreloadTime);
		}
	}

    public override void Reset()
    {
		base.Reset();
		currentAmmo = MAXAMMO;
		sprite.Rotation = new Vector2(-Position.X, -Position.Y).Angle();
    }

    public override void Start()
    {
		if (Main.IsDead) return;
        base.Start();
    }
	
	public LinearProjectile GetNewBullet()
	{
		LinearProjectile newBullet = bullet.Instantiate<LinearProjectile>();
		newBullet.owner = Main;
		newBullet.SetDirection(sprite.Rotation);
		newBullet.GlobalPosition = Main.GlobalPosition;
		newBullet.isInPilotArea = false;
		return newBullet;
	}
}
