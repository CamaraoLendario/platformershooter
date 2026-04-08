using Godot;
using System;

[Tool]
public partial class IceRayParticles : GpuParticles2D
{
	[Export] GpuParticles2D iceDropplets;
	[Export] GpuParticles2D vortex;

	[Export] float Dist
	{
		get
		{
			return dist;
		}
		set
		{
			dist = value;
			SetVars(dist, Dir);
		}
	}

	float dist = 100f;
	[Export] Vector2 Dir
	{
		get
		{
			return dir;
		}
		set
		{
			dir = value;
			SetVars(Dist, dir);
		}
	}
	Vector2 dir = Vector2.Right;

	void SetVars(float dist, Vector2 dir)
	{
		ShaderMaterial iceDroppletsMaterial = iceDropplets.ProcessMaterial as ShaderMaterial;
		ShaderMaterial vortexMaterial = vortex.ProcessMaterial as ShaderMaterial;

		iceDropplets.Amount = (int) (dist/5);
		iceDroppletsMaterial.SetShaderParameter("amount", iceDropplets.Amount);
		iceDroppletsMaterial.SetShaderParameter("dist", dist);
		
		vortex.Amount = (int) (dist * 2.4);
		vortexMaterial.SetShaderParameter("amount", vortex.Amount);
		
	}
	
}
