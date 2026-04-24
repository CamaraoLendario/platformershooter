using Godot;
using System;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class TextOptions : MenuItem
{
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

    public override bool OnPositiveAction()
	{
		currentOptionIdx = NormalizeIdx(currentOptionIdx + 1, options.Length);
		label.Text = options[currentOptionIdx];
		return true;
	}

}
