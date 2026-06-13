using Godot;
using System;

public partial class HoldBackProgressBar : ColorRect
{
	[Signal] public delegate void SuccessEventHandler();
	public bool isBeingHeld = false;
	float progress = 0;
	ShaderMaterial material;
    public override void _Ready()
	{
		material = Material as ShaderMaterial;
	}

    public override void _Process(double delta) {
		if (isBeingHeld)
			progress = Mathf.Clamp(progress + (float)delta, 0f, 1f);
		else
			progress = Mathf.Clamp(progress - (float)delta, 0f, 1f);
		
		material.SetShaderParameter("value", progress);
		
		if (progress >= 1) {
			EmitSignal(SignalName.Success);
			progress = 0;
			SetDeferred(PropertyName.isBeingHeld, false);
		}
	}
}
