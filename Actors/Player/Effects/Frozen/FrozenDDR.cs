using Godot;
using System;

public partial class FrozenDDR : Node2D
{
	[Export] DDRInputsParent DDRparent;
	[ExportGroup("Nodes")]
	[Export] GpuParticles2D explosionParticles;
	Player frozenPlayer; 
    public override void _Ready()
    {
		frozenPlayer = GetParent<Player>();
		DDRparent.frozenPlayer = frozenPlayer;
        DDRparent.ended += End;
	}

	async void End()
	{
		frozenPlayer.effectHandler.CallDeferred(PlayerEffectHandler.MethodName.UnFreeze);
		SummonDestroyedParticles();
		Hide();
		await ToSignal(GetTree().CreateTimer(0.21), Timer.SignalName.Timeout);
		QueueFree();
	}

	public void ForceEnd()
	{
		frozenPlayer.effectHandler.CallDeferred(PlayerEffectHandler.MethodName.UnFreeze);
		SummonDestroyedParticles();
		QueueFree();
	}

	void SummonDestroyedParticles()
	{
		GpuParticles2D newParticles = GPUParticlesPool.GetClonedParticles(explosionParticles);

		newParticles.Emitting = true;
		newParticles.Position = GlobalPosition;
		//newParticles.Show();
	}
}
