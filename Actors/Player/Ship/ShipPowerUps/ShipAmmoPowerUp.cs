using Godot;
using System;

public partial class ShipAmmoPowerUp : ShipPowerUp
{
	[Export] PackedScene NewAmmo;
	PackedScene defaultShot = GD.Load<PackedScene>("uid://ceiwo7xx53l5y");	// default Ship ammo
	[Export] int currentAmmo = 3;
	public override void _Ready()
    {
        base._Ready(); // <-- keep this, idiot
		controller.bullet = NewAmmo;
		controller.Shot += OnShoot;
    }
	void OnShoot()
	{
		currentAmmo -= 1;
		GD.Print(currentAmmo);	
		if (currentAmmo <= 0)
		{
			End();
		}
	}
    public override void End()
    {
		controller.bullet = defaultShot;
		controller.Shot -= OnShoot;
        base.End();
	}
}
