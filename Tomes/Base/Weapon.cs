using Godot;
using GodotPlugins.Game;
using System;
using System.Runtime.InteropServices.Marshalling;

public partial class Weapon : Node2D
{
	[Signal] public delegate void ShotEventHandler(); 
	[Export] public bool flips = true;
	[Export] public bool rotates = true;
	[Export] public Node2D sprite;
	[Export] protected PackedScene bullet;
	[Export] public int maxAmmo = 1;
	protected int currentAmmo;
	[Export] protected AudioStreamPlayer2D shootAudio;
	private Timer shootCooldownTimer = new();
	[Export] float shootCooldownTime = 0.1f;
	public Player owner;
	protected World world;
	protected PilotWeaponHolder holder;
	Map currentMap = Game.Instance.world.currentMap;

	public override void _Ready()
	{
		shootCooldownTimer.OneShot = true;
		AddChild(shootCooldownTimer);

		currentAmmo = maxAmmo;
		world = Game.Instance.world;
		holder = GetParent<PilotWeaponHolder>();
	}

	public virtual bool OnShoot(Vector2 inputDir)
	{
		if (!shootCooldownTimer.IsStopped())
		{
			return false;
		}
		shootCooldownTimer.Start(shootCooldownTime);
		GD.Print("shooting: ", this);
		AudioStreamPlayer2D newShootAudio = shootAudio.Duplicate() as AudioStreamPlayer2D;
		world.AddChild(newShootAudio);
		newShootAudio.PitchScale = 1 + (float)GD.RandRange(-0.1f, 0.1f);
		newShootAudio.Play();
		newShootAudio.Finished += () => newShootAudio.QueueFree();
		EmitSignal(SignalName.Shot);
		return true;
	}

	protected LinearProjectile GetNewBullet(int colorIdx, Vector2 inputDir)
	{
		LinearProjectile newBullet = bullet.Instantiate<LinearProjectile>();
		newBullet.owner = owner;
		newBullet.SetDirection(inputDir);
		newBullet.Position = holder.GlobalPosition;
		newBullet.isInPilotArea = currentMap.IsPositionInPilotArea(GlobalPosition);
		return newBullet;
	}
} 