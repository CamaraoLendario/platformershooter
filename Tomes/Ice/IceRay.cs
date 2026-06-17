using Godot;
using System.Collections.Generic;
public partial class IceRay : HitscanBullet
{
	[ExportGroup("Visual")]
	[Export] int animationSpeed = 1;
	[Export] int offsetInFrontOfCharacter = 13;
	[ExportGroup("Nodes")]
	[Export] public GpuParticles2D hitEmitter;
	public override void _Ready()
	{
 		base._Ready();
		GpuParticles2D iceDropplets = GetNode<GpuParticles2D>("IceDropplets");
		GpuParticles2D vortex = GetNode<GpuParticles2D>("Vortex");

		Vector2 colPos = GetCollisionPoint();
		float distance = (colPos - GlobalPosition).Length();

		iceDropplets.Position += inputDir * offsetInFrontOfCharacter;
		iceDropplets.Amount = Mathf.Max(1, ((int)distance / 5) - offsetInFrontOfCharacter / 5);
		vortex.Position += inputDir * offsetInFrontOfCharacter;
		vortex.Amount = Mathf.Max(1, ((int)distance * 3) - offsetInFrontOfCharacter * 3);
		SetParticlesProperties(iceDropplets, distance, inputDir);
		SetParticlesProperties(vortex, distance, inputDir);
		hitEmitter.GlobalPosition = colPos;
		(hitEmitter.ProcessMaterial as ParticleProcessMaterial).Direction = -new Vector3(inputDir.X, inputDir.Y, 0f);

		iceDropplets.Restart();
		vortex.Restart();
		hitEmitter.Restart();
 	}

	void SetParticlesProperties(GpuParticles2D particlesShader, float distance, Vector2 inputDir)
	{
		ShaderMaterial shaderMaterial = particlesShader.ProcessMaterial as ShaderMaterial;
		shaderMaterial.SetShaderParameter("dir", inputDir);
		shaderMaterial.SetShaderParameter("dist", distance);
	}

	protected override void Hit(Player player)
	{
		if(player.hasShield)
			player.TakeDamage(owner);
		else
			player.effectHandler.Freeze();
	}
}