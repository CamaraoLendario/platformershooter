using Godot;
using SpaceMages;
using Microsoft.VisualBasic;
using System;
using System.Linq;
public partial class IndividualScoreCounter : Control
{
	[Export] Label name;
	[Export] Label scoreCount;
	[Export] Label addedScore;
	public int colorIdx = -1;
	Tween posTween;
	Tween alphaTween;
	int lastAddedScore = 0;

    public override void _Ready()
	{
		GD.Print("MY COLOR INDEX IS " + colorIdx);
		name.Text = SpaceMagesVars.teamColorsDict.Keys.ElementAt(colorIdx);
    }

	void SetScore(int score)
	{
		scoreCount.Text = score.ToString();
	}

	public void AddScore(int score = 1)
	{
		if (score == 0) return;
		else if (score < 0)
			addedScore.Text = "-" + score;
		else addedScore.Text = "+" + score;
		lastAddedScore = score;

		posTween = CreateAddedScoreTween();
		alphaTween = CreateAddedScoreTween();
		alphaTween.Finished += OnAnimationFinished;

		addedScore.Modulate = new Color(1, 1, 1, 1);

		addedScore.CustomMinimumSize = new Vector2(69, 17);

		posTween.TweenMethod(Callable.From((float Ypos) => SetAdderPosition(Ypos)), 50, 0, 0.6f);
		alphaTween.TweenProperty(addedScore, "modulate:a", 0f, 0.6f);
	}

	void SetAdderPosition(float Ypos)
	{
		addedScore.GlobalPosition = (Vector2.Down * Ypos) + scoreCount.GlobalPosition;
	}

	void OnAnimationFinished()
	{
		scoreCount.Text = (scoreCount.Text.ToInt() + lastAddedScore).ToString();
		GD.Print((scoreCount.Text.ToInt() + lastAddedScore).ToString());
		alphaTween.Finished -= OnAnimationFinished;
	}
	
	Tween CreateAddedScoreTween()
    {
		Tween tween = CreateTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.SetTrans(Tween.TransitionType.Sine);
		return tween;
    }
}