using Godot;
using System;

public partial class PilotWeaponHolder : Node2D
{
	[Signal] public delegate void WeaponShotEventHandler();
	[Export] PilotAttack Pilot;
	[Export] Sprite2D crossAirSprite;

	public Weapon CurrentWeapon
	{
		get
		{
			return currentWeapon;
		}
		set
		{
			if (currentWeapon != null || value == null){ return; }
			currentWeapon =	value;
			InitializeWeapon();
		}
	}
	Weapon currentWeapon = null;
	World world;

    public override void _Ready()
    {
		crossAirSprite.Hide();
		world = Game.Instance.world;
		Pilot.playerInput.DropStart += DropWeapon;
    }
	public override void _Process(double delta)
	{
		crossAirSprite.Visible = Pilot.Main.isAiming;
		Rotation = Pilot.aimVector.Angle();
		if (CurrentWeapon == null) return;
		
		if(!CurrentWeapon.rotates) CurrentWeapon.Rotation = -Pilot.aimVector.Angle();

		if (Pilot.aimVector.X < 0 && CurrentWeapon.flips)
			FlipVSprite(CurrentWeapon.sprite, true);
		else FlipVSprite(CurrentWeapon.sprite, false);
	}
	void InitializeWeapon()
	{
		currentWeapon.owner = Pilot.Main;
		CallDeferred(MethodName.AddChild, currentWeapon);
    }
	public void DropWeapon()
	{
		if (currentWeapon == null) return;
		currentWeapon.QueueFree();
		currentWeapon = null;
		world.UpdateWeaponPickups();
    }
	public void Shoot(Vector2 inputdir)
	{
		if (CurrentWeapon != null)
		{
			EmitSignal(SignalName.WeaponShot);
			CurrentWeapon.OnShoot(inputdir);
		}
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
}