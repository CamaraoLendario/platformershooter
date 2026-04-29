using Godot;
using System;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class TextOptions : MenuItem
{
	[Export] bool usesHorizontalDir = false;
	[Export] string[] Options
	{
		get
		{
			return options;
		}
		set
		{
			options = value;
			if (label != null) label.Text = options[0];
		}
	}
	string[] options = [
		"On",
		"Off"
	];
	int currentOptionIdx = 0;
	[Export] Label label;

    public override void _Ready()
	{
        label.Text = options[currentOptionIdx];
	}

    public override bool OnInteract()
	{
		SetOptionIdx(currentOptionIdx + 1);
		return true;
	}

    public override bool OnMoveAction(Vector2 dir)
    {
		if (!usesHorizontalDir) return false;
		SetOptionIdx(currentOptionIdx + (int)dir.X);
        return true;
    }

	void SetOptionIdx(int index)
	{
		currentOptionIdx = NormalizeIdx(index, options.Length);
		label.Text = options[currentOptionIdx];
	}
}
