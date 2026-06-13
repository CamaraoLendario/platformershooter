using Godot;
using System;
using System.Net;

public partial class DrillShot : LinearProjectile
{
	const int MAXSPEED = 500;

    public override void _PhysicsProcess(double delta)
    {
		CheckForDestroyTiles();
        base._PhysicsProcess(delta);
    }

	protected override void Move(double delta)
    {
		if (isColiding)
		{
			speed = MAXSPEED/4;
		}
		else
		{
			speed += (float)delta * MAXSPEED;
			if (speed < MAXSPEED)
				speed = MAXSPEED;
		}
		
		base.Move(delta);
    }

}