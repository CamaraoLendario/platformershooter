using Godot;
using System;

public partial class PlayerParticleEmitters : Node2D
{
	public override void _Ready()
	{
		(GetChild(0) as GpuParticles2D).Finished += OnFinished;
	}

	public void Emit()
    {
		foreach (GpuParticles2D particles in GetChildren())
		{
			particles.Emitting = true;
		}
    }
	
	void OnFinished(){ QueueFree(); }
}
