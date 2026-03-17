using Godot;
using System;

public partial class DebugCamera : Camera2D
{
    public override void _Ready()
    {
        MakeCurrent();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsMouseButtonPressed(MouseButton.WheelDown))
		{
			Zoom -= Vector2.One * 0.1f;
		}
		if (Input.IsMouseButtonPressed(MouseButton.WheelUp))
		{
			Zoom += Vector2.One * 0.1f;
		}
    }

}
