using Godot;
using System;

public partial class Test : Node2D
{
    public override void _PhysicsProcess(double delta)
	{
		GD.Print("runningPhysics");
	}
}
