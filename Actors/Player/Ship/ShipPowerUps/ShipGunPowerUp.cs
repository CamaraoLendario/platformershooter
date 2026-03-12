using Godot;
using System;

public partial class ShipGunPowerUp : ShipPowerUp
{
	public override void _Ready()
    {
        base._Ready(); // <-- keep this, idiot
		controller.Shot += Shoot;
    }
    public override void End()
    {
        base.End();
		controller.Shot -= Shoot;
	}

	protected virtual void Shoot()
	{
		GD.PrintErr("Gun Power Up does not have a 'Shoot()' override or is calling this function");
	}
	
}
