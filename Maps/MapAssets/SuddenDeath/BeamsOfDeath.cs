using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class BeamsOfDeath : Node2D
{
	[Export] float beamVelocity = 10;
	[ExportGroup("Nodes")]
	[Export] Area2D beamRight;
	[Export] Area2D beamLeft;
	Map currentMap;
	Timer SuddenDeathTimer = new();
	int started = 0;
	List<(Area2D beam, Vector2 beamPos)> beams = [];

    public override void _Ready()
    {
        beams.Add((beamLeft, beamLeft.Position));
		beams.Add((beamRight, beamRight.Position));

		currentMap = GetParent<Map>();
		AddChild(SuddenDeathTimer);
		SuddenDeathTimer.OneShot = true; 
		SuddenDeathTimer.Timeout += () =>
		{
			started = 1;
		};

		SuddenDeathTimer.Start(currentMap.TimeToSuddenDeath);
		Game.Instance.NewRoundStarted += Reset;
    }

    public override void _Process(double delta)
	{
		Vector2 moveVec = beamVelocity * (float)delta * Vector2.Right * started;
		beamRight.Position += -moveVec;
		beamLeft.Position += moveVec;

		foreach ((Area2D beam, Vector2 beamPos) beam in beams)
		{
			foreach (Node2D body in beam.beam.GetOverlappingBodies())
			{
				if (body is not Player player) return;

				player.TakeDamage(player.colorIdx);
			}
		}
	}

	void Reset()
	{
		foreach ((Area2D beam, Vector2 beamPos) beam in beams)
		{
			beam.beam.Position = beam.beamPos;
			started = 0;
		}
		SuddenDeathTimer.Start(currentMap.TimeToSuddenDeath);
	}

    public override void _ExitTree()
    {
		Game.Instance.NewRoundStarted -= Reset;
        base._ExitTree();
    }

}
