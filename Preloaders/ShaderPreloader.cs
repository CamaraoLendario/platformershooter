using Godot;
using System;
using System.Collections.Generic;

public partial class ShaderPreloader : Node2D
{
	[Export] Shaders2DPreloader shaders2DPreloader;
	[Export] ShadersParticlesPreloader shadersParticlesPreloader;

	bool isShaderCompilationComplete = false;
	bool isParticlesShaderCompilationComplete = false;

    public override void _Ready()
    {
		shaders2DPreloader.Shaders2DPreloadingFinished += () =>
		{
			isShaderCompilationComplete = true;
			CheckCompilationState();
		};
		shadersParticlesPreloader.ShadersParticlesPreloadingFinished += () =>
		{
			isParticlesShaderCompilationComplete = true;
			CheckCompilationState();
		};
		shaders2DPreloader.CallDeferred(Shaders2DPreloader.MethodName.BeginPreloadingShaders);
		shadersParticlesPreloader.CallDeferred(ShadersParticlesPreloader.MethodName.BeginPreloadingParticles);
	}

	void CheckCompilationState()
	{
		if (!(isShaderCompilationComplete && isParticlesShaderCompilationComplete)) return;

		GD.Print("Shaders compilation complete!");

		QueueFree();
	}
}
