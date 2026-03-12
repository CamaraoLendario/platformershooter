using Godot;
using System;

public partial class ItemPickupNotification : Node2D
{
	[Export] float animationLength = 1;
	float time = 0;
	Vector2 originalPos;

    public override void _Ready()
    {
        originalPos = Position + Vector2.Up * 20;
    }


	public void Setup(string pickupName, Color pickupColor)
	{
		Label text = GetChild<Label>(0);
		text.Text = "+ " + pickupName;
		Modulate = pickupColor;
	}

    public override void _Process(double delta)
	{
		time += (float)delta / animationLength;
		float value = Mathf.Sin((time - 1) * Mathf.Pi/2f) + 1;

		Position = originalPos + Vector2.Up * value * 30;
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1 - value);

		if (time > 1)
			QueueFree();
	}

}
