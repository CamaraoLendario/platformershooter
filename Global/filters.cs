using Godot;
using System;

public partial class filters : ColorRect
{
	bool isEnabled = true;
    public override void _Input(InputEvent @event)
	{
		if (Input.IsKeyLabelPressed(Key.M))
		{
			(Material as ShaderMaterial).SetShaderParameter("enabled", !isEnabled);
			isEnabled = !isEnabled;
		}
	}

}
