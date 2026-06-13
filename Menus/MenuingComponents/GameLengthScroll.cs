using Godot;
using System;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class GameLengthScroll : MenuScroll
{
	public override void _Ready()
    {
		base._Ready();
        Contents = Enum.GetNames(typeof(GamemodeLogic.GameLength));
		SetIdx(1);
    }
	
	string GetGamemodeString(GamemodeLogic.GameLength gamemodeIdx)
	{
		switch (gamemodeIdx)
		{
			case GamemodeLogic.GameLength.Short:
				return "Short";
			case GamemodeLogic.GameLength.Medium:
				return "Medium";
			case GamemodeLogic.GameLength.Long:
				return "Long";
		}
		return "";
	}

    protected override void SetLabels(int dir)
    {
        for(int i = 0; i < 4; i++)
		{
			int value = (dir - 1)/2;
			int labelGMIdxOffset = - 2 - value;

			Label currentLabel = labelsContainer.GetChild(i) as Label;
			
			currentLabel.Text = GetGamemodeString((GamemodeLogic.GameLength)NormalizeIdx(i + currentIdx + labelGMIdxOffset , contents.Length));
			currentLabel.Size *= Vector2.Down;
		}	
    }


	public void SetGamemodeLength(){
		Game.GetGamemodeLogic().CurrentGameLength = (GamemodeLogic.GameLength)currentIdx;
	}
}
