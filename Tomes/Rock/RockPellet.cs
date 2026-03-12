using Godot;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;

public partial class RockPellet : LinearProjectile
{
	[Export] float rotPerSec = 15f;
    List<Player> playerExceptions = new();
    public List<RockPellet> sisterPellets;
	int rotDir;
    public override void _Ready()
    {
        base._Ready();

		rotDir = GD.RandRange(0, 1);
		rotDir *= 2;
		rotDir -= 1;

		speed *= (float) GD.RandRange(0.9, 1.1);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
		Rotation += rotDir * (float)delta * rotPerSec * Mathf.Pi * 2;
    }

    protected override void OnBodyHit(Node2D body)
    {
        if (playerExceptions.Contains(body)) return;
        base.OnBodyHit(body);
        if (body is Player player)
        {
            foreach(RockPellet sisterPallet in sisterPellets)
            {
                sisterPallet?.playerExceptions.Add(player);
            }
        }
    }

}
