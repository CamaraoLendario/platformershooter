using Godot;
using System;

public partial class ExperimentalFeatures : Control
{
	[Export] Label label;
	public bool isActivated = true;
	public float interval = 10f;

    public override void _Ready()
    {
        base._Ready();
		Game.Instance.experimentalFeatures = this;
    }

	void Update()
	{
		string activatedtext = "false";
		if (isActivated) activatedtext = "true";

		label.Text = $"Magic missiles: {activatedtext}\nInterval: {interval}s";
	}

    public override void _Input(InputEvent @event)
	{
		if (Input.IsKeyLabelPressed(Key.O))
		{
			interval ++;
			Update();
		}
		else if (Input.IsKeyLabelPressed(Key.L))
		{
			interval --;
			Update();
		}
		else if (Input.IsKeyLabelPressed(Key.I))
		{
			isActivated = !isActivated;
			Update();
		}
	}
}
