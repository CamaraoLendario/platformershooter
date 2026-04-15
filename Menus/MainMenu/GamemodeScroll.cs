using Godot;
using System.Collections.Generic;
using static SpaceMages.SpaceMagesVars;

public partial class GamemodeScroll : Control
{
	[Export] string[] contents = [];
	[ExportGroup("Nodes")]
	[Export] AnimationPlayer scrollAnimator;
	[Export] Control panelHelper;
	int currentIdx = 0;
	string scrollDown = "scrollDown";
	string scrollUp = "scrollUp";
	(Vector2 size, Vector2 position) openHelperSettings = (
		new Vector2(385.0f, 287.0f),
		new Vector2(-1.5f, 77.0f)
	);
	(Vector2 size, Vector2 position) closedHelperSettings = (
		new Vector2(385.0f, 95.0f),
		new Vector2(-1.5f, 173.0f)
	);

    public override void _Ready()
    {
        currentIdx = 0;
		SetLabels(1);
    }

	public string Scroll(int direction)
	{
		string scrollDir = scrollDown;
		if (direction == 1){
			scrollDir = scrollUp;
		}

		currentIdx = NormalizeIdx(currentIdx + direction, contents.Length);

		SetLabels(direction);
		skipAnimation();
		scrollAnimator.Play(scrollDir);
		return contents[currentIdx];
	}

	void SetLabels(int dir)
	{
		for(int i = 0; i < 4; i++)
		{
			int value = (dir - 1)/2;
			int labelGMIdxOffset = - 2 - value;

			Label currentLabel = GetChild(i) as Label;
			
			currentLabel.Text = contents[NormalizeIdx(i + currentIdx + labelGMIdxOffset , contents.Length)];
		}
	}

	public void Open()
	{
		for(int i = 0; i < 4; i++)
		{
			GetChild<Label>(i).Visible = true;
		}

		panelHelper.Position = openHelperSettings.position;
		panelHelper.Size = openHelperSettings.size;
	}
	public void Close()
	{
		int currentLabelIdx = 1;
		if (scrollAnimator.CurrentAnimation == scrollDown){
			currentLabelIdx = 2;
		}

		for(int i = 0; i < 4; i++)
		{
			if (i == currentLabelIdx) continue;
			GetChild<Label>(i).Visible = false;


		}

		panelHelper.Position = closedHelperSettings.position;
		panelHelper.Size = closedHelperSettings.size;
		skipAnimation();
	}

	void skipAnimation()
	{
		if (scrollAnimator.IsPlaying()){
			scrollAnimator.Seek(scrollAnimator.CurrentAnimationLength);
		}
	}
}
