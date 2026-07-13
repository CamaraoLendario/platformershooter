using Godot;
using System;

[Tool]
public partial class ParticlesTesting : GpuParticles2D
{
	[Export] bool enabled = true;
	float time = 0f;
	public override void _Process(double delta)
	{
		if (enabled)
		{
			time += (float)delta *2f;
			Position = new Vector2(MathF.Cos(time) * 100f, 0);
		}
		else time = 0f;
	}
}
