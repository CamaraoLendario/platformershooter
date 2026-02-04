using Godot;
using System;

public partial class PlayeronPlayerCollision : Area2D
{
	public Player main;
	bool isColliding = false;
    public override void _Ready()
    {
		main = GetParent() as Player;
        AreaEntered += OnAreaEntered;
    }

	void OnAreaEntered(Area2D area)
	{
		if (area is PlayeronPlayerCollision playercol)
		{
			isColliding = true;
		}
	}

    public override void _PhysicsProcess(double delta)
    {
        if (!isColliding) return;
		
		bool collidedWithPlayer = false;

		foreach (Node2D area in GetOverlappingAreas())
		{
			if (area is PlayeronPlayerCollision playercol)
			{
				main.Velocity += (main.Position - playercol.main.Position).Normalized() * PilotController.MAXHSPEED/10;
				GD.Print("playersCollided"); 
				collidedWithPlayer = true;
			}
    	}

		if (!collidedWithPlayer)
		{
			isColliding = false;
		}
	}
}
