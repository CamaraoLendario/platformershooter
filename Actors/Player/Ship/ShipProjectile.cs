using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

public partial class ShipProjectile : LinearProjectile
{
	[ExportGroup("Nodes")]
	[Export] GpuParticles2D trailParticles;
	[Export] GpuParticles2D explosionParticles;
	PointLight2D projectileLight;
    public override void _Ready()
    {
        base._Ready();
		projectileLight = GetNode<PointLight2D>("%ProjectileLight");
		(trailParticles.ProcessMaterial as ShaderMaterial).SetShaderParameter("projectileDir", direction);
    }

    public override bool End(EndingReason endingReason, bool allowFreeing = true, bool forceFreeing = false)
    {
		if (!base.End(endingReason, false)) return false;
		ending = true;
        speed = 0;
		SetDeferred(PropertyName.Monitoring, false);
		SetDeferred(PropertyName.Monitorable, false);
		trailParticles.Emitting = false;
		SpawnExplosionParticles();
		PrepareForDeletion();
		return true;
    }

    private void SpawnExplosionParticles()
	{
		GpuParticles2D newParticles = explosionParticles.Duplicate() as GpuParticles2D;
		newParticles.Position = GlobalPosition;
		newParticles.Emitting = true;
		newParticles.Finished += newParticles.QueueFree;
		Game.Instance.CallDeferred(MethodName.AddChild, newParticles);
	}

	async void PrepareForDeletion()
	{
		trailParticles.Emitting = false;
		sprite.SelfModulate = new Color(0f, 0f, 0f, 0f);
		Tween tween = CreateTween();
		tween.TweenMethod(Callable.From((float tweenedValue) =>
		{
			projectileLight.Energy = tweenedValue;
		}), 1f, 0f, trailParticles.Lifetime/10f);
		await ToSignal(GetTree().CreateTimer(trailParticles.Lifetime), Timer.SignalName.Timeout);
		QueueFree();
	}	
}
