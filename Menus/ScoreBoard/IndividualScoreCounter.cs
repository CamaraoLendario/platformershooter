using Godot;
using static SpaceMages.SpaceMagesVars;
using Microsoft.VisualBasic;
using System;
using System.Linq;
[Tool]
public partial class IndividualScoreCounter : MenuItem
{
	[ExportToolButton("Add 1 Score")]
	public Callable AddAScore => Callable.From(AddScore);
	[ExportToolButton("Remove 1 Score")]
	public Callable RemoveAScore => Callable.From(RemoveScore);
	[Export] public float animationTime = 1f;
	string name;
	public int colorIdx = -1;
	int lastAddedScore = 0;
	int currentScore = 0;
	ShaderMaterial material;
	const string COUNTERSCENEUID = "uid://fg3evf2er64m";

    public override void _Ready()
	{
		Label label = GetNode<Label>("%nameLabel");
		TextureRect txtr = GetNode<TextureRect>("%crowdTexture"); 
		
		label.Text = name;
		
		material = txtr.Material as ShaderMaterial;
		material.SetShaderParameter("pointsValue", 0f);
		#if TOOLS
			if (Engine.IsEditorHint()){
				material.SetShaderParameter("teamColor", teamColors[0]);
				return;
			}
		#endif
		material.SetShaderParameter("teamColor", teamColors[colorIdx]);
	}

	public static IndividualScoreCounter New(PlayerInfo playerInfo)
	{
		return New(playerInfo.Name, playerInfo.colorIdx);
	}

    public static IndividualScoreCounter New(string name, int colorIdx)
	{
		IndividualScoreCounter newScoreCounter = GD.Load<PackedScene>(COUNTERSCENEUID).Instantiate<IndividualScoreCounter>();
		newScoreCounter.name = name;
		newScoreCounter.colorIdx = colorIdx;

		return newScoreCounter;
	} 

    void SetScore(int score)
	{
		currentScore = score;
		material.SetShaderParameter("pointsValue", (float)score/Game.GetGamemodeLogic().GetWinningScore());
	}
	void RemoveScore()
	{
		AddScore(-3);
	}
	void AddScore()
	{
		AddScore(3);
	}
	public void AddScore(int score)
	{
		int necessaryScore = Game.GetGamemodeLogic().GetWinningScore();
	
		currentScore += score;
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.Out);
		float oldTweenedValue = 0;
		tween.TweenMethod(Callable.From((float tweenedValue) =>{
			material.SetShaderParameter("pointsValue", ((float)(currentScore - score) + (float)(score * tweenedValue))/necessaryScore);
			material.SetShaderParameter("offsetStrength", (tweenedValue - oldTweenedValue)/GetProcessDeltaTime());
			oldTweenedValue = tweenedValue;
		}), 0f, 1f, animationTime);

	}
}