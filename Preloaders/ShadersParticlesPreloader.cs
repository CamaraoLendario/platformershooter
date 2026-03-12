using Godot;
using System;

public partial class ShadersParticlesPreloader : GpuParticles2D
{
	[Signal] public delegate void ShadersParticlesPreloadingFinishedEventHandler();
	[Export] ParticleProcessMaterial[] particlesToPreload =
	{// holy pirate software
		GD.Load<ParticleProcessMaterial>("uid://iawq5go2txhe"),		// Dash Particles
		GD.Load<ParticleProcessMaterial>("uid://x38tv3nmu01"),		// Pilot Shield Break
		GD.Load<ParticleProcessMaterial>("uid://b5s0ni7p05ow1"),	// Ship Shield Break
		GD.Load<ParticleProcessMaterial>("uid://bb6qj2nxuidly"),	// Ship Projectile Explosion
		GD.Load<ParticleProcessMaterial>("uid://bniqjtc2x7lkc"),	// Ship Projectile Trail
		GD.Load<ParticleProcessMaterial>("uid://evaeqexklt5o"),		// Fire Projectile Explosion
		GD.Load<ParticleProcessMaterial>("uid://biflh5l4gvpao"),	// Fire Projectile Trail
		GD.Load<ParticleProcessMaterial>("uid://36slk4uiaeax"),		// Fire Tome Trail
		GD.Load<ParticleProcessMaterial>("uid://um8eg4sda5mp"),		// Ice Dropplets
		GD.Load<ParticleProcessMaterial>("uid://dbwnxxkulxaro"),	// Ice Hit
		GD.Load<ParticleProcessMaterial>("uid://bb5cmnqanosm4"),	// Ice Rings
		GD.Load<ParticleProcessMaterial>("uid://dhsdia54awj6f"),	// Poison Mine Explosion
	};
	ShaderPreloader Main;
    public override void _Ready()
	{
		Main = GetParent<ShaderPreloader>();
	}

	public async void BeginPreloadingParticles()
	{
		
		foreach(ParticleProcessMaterial particleMaterial in particlesToPreload)
		{
			ProcessMaterial = particleMaterial;
			Emitting = true;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
		CallDeferred(MethodName.EmitSignal, SignalName.ShadersParticlesPreloadingFinished);
	} 
}
