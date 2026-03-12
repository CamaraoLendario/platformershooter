using Godot;
using System;

public partial class Shaders2DPreloader : ColorRect
{
	[Signal] public delegate void Shaders2DPreloadingFinishedEventHandler();
	[Export] Shader[] shadersToPreload =
	{
		GD.Load<Shader>("uid://cywx8daesh6uu"),	//OutlineShader
		GD.Load<Shader>("uid://cx67s33dt1lle"),	//HomemadeParallax
		GD.Load<Shader>("uid://djdkl0hm4q5m6"),	//BorderShader
		GD.Load<Shader>("uid://dha1qnh4th4vi"),	//SelectedMapPreview
	};
	ShaderPreloader Main;
    public override void _Ready()
	{
		Main = GetParent<ShaderPreloader>();
	}

	public async void BeginPreloadingShaders()
	{
		ShaderMaterial testMaterial = new ShaderMaterial();
		Material = testMaterial;
		foreach(Shader shader in shadersToPreload)
		{
			testMaterial.Shader = shader;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
		CallDeferred(MethodName.EmitSignal, SignalName.Shaders2DPreloadingFinished);
	} 
}
