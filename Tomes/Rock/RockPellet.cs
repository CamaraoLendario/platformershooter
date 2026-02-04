using Godot;
using System;
using System.IO.Pipes;

public partial class RockPellet : LinearProjectile
{
	[Export] float rotPerSec = 15f;
	int rotDir;
    public override void _Ready()
    {
        base._Ready();

		rotDir = GD.RandRange(0, 1);
		rotDir *= 2;
		rotDir -= 1;

		velocity *= (float) GD.RandRange(0.9, 1.1);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
		Rotation += rotDir * (float)delta * rotPerSec * Mathf.Pi * 2;
    }
}
