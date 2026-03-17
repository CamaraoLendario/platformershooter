using Godot;
using System;
using System.ComponentModel;

[Tool]
public partial class DEBUGRADIALMENU : Node2D
{
	[Export] bool isShowing = true;
	[Export] int MenuSectionCount = 8;

	[ExportCategory("Circle Settings")]
	[Export] float Radius = 32;
	[Export] float borderThickness = 1;
	[Export] Color borderColor = new Color(1, 1, 1);
	[Export] Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
	[Export] bool antiAliased = true;

	[Export] int currentSelectedItem = 0;

	void OpenMenu()
	{
		isShowing = true;
	}
	void CloseMenu()
	{
		isShowing = false;
	}


    public override void _Process(double delta)
    {
	    QueueRedraw();
    }

    public override void _Draw()
    {
		if (!isShowing) return;
		DrawCircle(Vector2.Zero, Radius, backgroundColor, true, -1, false);
		Vector2[] points = [
			Vector2.Zero,
			Vector2.Up.Rotated((currentSelectedItem + 0.5f) * (Mathf.Pi * 2 / MenuSectionCount)) * Radius,
			Vector2.Up.Rotated((currentSelectedItem + 0.75f) * (Mathf.Pi * 2 / MenuSectionCount)) * Radius,
			Vector2.Up.Rotated((currentSelectedItem + 1.0f) * (Mathf.Pi * 2 / MenuSectionCount)) * Radius,
			Vector2.Up.Rotated((currentSelectedItem + 1.25f) * (Mathf.Pi * 2 / MenuSectionCount)) * Radius,
			Vector2.Up.Rotated((currentSelectedItem + 1.5f) * (Mathf.Pi * 2 / MenuSectionCount)) * Radius
		];
		DrawColoredPolygon(points, backgroundColor);
        DrawCircle(Vector2.Zero, Radius, borderColor, false, borderThickness, antiAliased);
		for(int i = 0; i < MenuSectionCount; i++)
		{
			Vector2 dir = Vector2.Up;
			dir = dir.Rotated((i + 0.5f) * (Mathf.Pi * 2 / MenuSectionCount));
			DrawLine(Vector2.Zero, dir * Radius, borderColor, borderThickness/1.5f, antiAliased);
		}	
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
		{
			GD.Print(mouseMotion.Velocity);
		}
    }
}
