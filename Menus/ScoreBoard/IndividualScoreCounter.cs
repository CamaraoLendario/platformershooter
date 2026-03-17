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
	[Export] AnimationPlayer animationPlayer;
	public int colorIdx = -1;
	int lastAddedScore = 0;

    public override void _Ready()
	{
		GD.Print("MY COLOR INDEX IS " + colorIdx);
		name.Text = SpaceMagesVars.teamColorsDict.Keys.ElementAt(colorIdx);
		animationPlayer.AnimationFinished += OnAnimationFinished;
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

		animationPlayer.Play("AddScore");
	}

	void OnAnimationFinished(StringName animName)
	{
		addedScore.Modulate = new Color(1, 1, 1, 0);
		scoreCount.Text = (scoreCount.Text.ToInt() + lastAddedScore).ToString();
		GD.Print((scoreCount.Text.ToInt() + lastAddedScore).ToString());
	}
}