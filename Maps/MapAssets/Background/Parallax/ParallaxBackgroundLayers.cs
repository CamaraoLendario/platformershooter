using Godot;
using System;

public partial class ParallaxBackgroundLayers : Node2D
{
	Camera2D currentCamera;
    public override void _Ready()
	{
		CallDeferred(MethodName.SetCamera);
	}

    void SetCamera()
	{
        currentCamera = GetParent<Map>().camera;
		foreach(HomemadeParallax child in GetChildren())
		{
			child.currentCamera = currentCamera;
		}
    }

	public override void _Process(double delta)
	{
		if (!Engine.IsEditorHint())
			Position = currentCamera.Position;
	}	
}
