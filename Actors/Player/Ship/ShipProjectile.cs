using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

public partial class ShipProjectile : LinearProjectile
{
	[ExportGroup("Nodes")]
	[Export] GpuParticles2D particlesTrail;
	[Export] GpuParticles2D explosionParticles;
    public override void _Ready()
    {
        base._Ready();

		(particlesTrail.ProcessMaterial as ParticleProcessMaterial).Direction = new Vector3(-direction.X, -direction.Y, 0);
    }

    public override void End()
    {
        velocity *= 0;
		SetDeferred(PropertyName.Monitoring, false);
		SetDeferred(PropertyName.Monitorable, false);

		sprite.Visible = false;
		particlesTrail.Emitting = false;
		SpawnExplosionParticles();
		WaitForParticlesEnd();
    }

    private void SpawnExplosionParticles()
	{
		GpuParticles2D newParticles = explosionParticles.Duplicate() as GpuParticles2D;
		newParticles.Position = GlobalPosition;
		newParticles.Emitting = true;
		newParticles.Finished += newParticles.QueueFree;
		Game.Instance.CallDeferred(MethodName.AddChild, newParticles);
	}


    private async void WaitForParticlesEnd()
    {
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		QueueFree();
    }

}
