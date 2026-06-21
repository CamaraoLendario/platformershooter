using Godot;
using System;

public partial class Shaders2DPreloader : ColorRect
{
	[Signal] public delegate void Shaders2DPreloadingFinishedEventHandler();
	[Export] ShaderMaterial[] shadersToPreload =
	{
		GD.Load<ShaderMaterial>("uid://b440sqhrft0wb"), // Pilot After Images
		GD.Load<ShaderMaterial>("uid://b5i1uqnrt70x5"), // Ship After Images
		GD.Load<ShaderMaterial>("uid://bvc3mf3avwkhd"), // Contestant Spawn Animation
		GD.Load<ShaderMaterial>("uid://6b8p1p0j0p4f"),  // Outline Material
		GD.Load<ShaderMaterial>("uid://cuu7ef3oi50rv"), // Pilot Death Particles
		GD.Load<ShaderMaterial>("uid://bgglq6jh35xxs"), // Ship Reconstruction Particles
		// GD.Load<Shader>("uid://cx67s33dt1lle"),	//HomemadeParallax
		// GD.Load<Shader>("uid://djdkl0hm4q5m6"),	//BorderShader
	};
	ShaderPreloader Main;
    public override void _Ready()
	{
		Main = GetParent<ShaderPreloader>();
	}

	public async void BeginPreloadingShaders()
	{
		foreach(ShaderMaterial shaderMaterial in shadersToPreload)
		{
			Material = shaderMaterial;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
		PreloadParallax();
		CallDeferred(MethodName.EmitSignal, SignalName.Shaders2DPreloadingFinished);
	}
	
	void PreloadParallax()
	{
		Material = new ShaderMaterial()
		{
			Shader = GD.Load<Shader>("uid://cx67s33dt1lle")
		};
	}
}
