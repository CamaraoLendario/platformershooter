using Godot;
using System.Collections.Generic;
using System.ComponentModel;
using static SpaceMages.SpaceMagesVars;
[Tool]
public partial class HorizontalTextAndText : HorizontalMenu
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
			if (rightLabel != null) rightLabel.Text = options[0];
		}
	}
	string[] options = [
		"On",
		"Off"
	];

	int currentOptionIdx = 0;

	Label rightLabel = null;

    public override void _Ready()
    {
		base._Ready();
		rightLabel = GetNode<Label>("rightLabel");
        rightLabel.Text = options[currentOptionIdx];
		

		CallDeferred(MethodName.DoFormatting);
		originalPosition = Position;
    }

    public override bool OnPositiveAction()
    {
		currentOptionIdx = NormalizeIdx(currentOptionIdx + 1, options.Length);
		rightLabel.Text = options[currentOptionIdx];
		return true;
    }
	protected override void DoFormatting()
	{
		base.DoFormatting();

		if (rightLabel == null){
			rightLabel = GetNode<Label>("rightLabel");
		}

		Vector2 BordedTotalSize = TotalSize - (Vector2.Right * borderSize);
		
		rightLabel.Size = new Vector2(leftLabel.Size.X, BordedTotalSize.Y);
		rightLabel.Position = (BordedTotalSize * new Vector2(0.5f, -0.5f)) - rightLabel.Size * Vector2.Right;
	}
}
