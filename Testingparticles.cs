using Godot;
using System;

[Tool]
public partial class Testingparticles : Node2D
{
	[Export] GpuParticles2D reconstruction;

	public override void _Process(double delta)
	{
		(reconstruction.ProcessMaterial as ShaderMaterial).SetShaderParameter("newPos", reconstruction.Position);
	}


}
