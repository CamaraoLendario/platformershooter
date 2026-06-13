using Godot;
using System;

[Tool]
public partial class rotateDEBUG : AnimatedSprite2D
{
	[Export] float wideness = 30f;
	Vector2 firstPos;
	float time = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		firstPos = Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Rotation += (float)delta;
		Rotation %= Mathf.Pi*2f;
		time += (float)delta*1f;
		time %= Mathf.Pi*2f;
		Position = firstPos +( Vector2.Right * Mathf.Sin(time) * wideness);
	}
}
