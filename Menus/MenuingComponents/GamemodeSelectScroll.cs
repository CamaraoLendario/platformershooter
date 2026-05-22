using Godot;
using System;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class GamemodeSelectScroll : MenuScroll
{
    public override void _Ready()
    {
		base._Ready();
        Contents = Enum.GetNames(typeof(Game.GamemodeIdxs));
	}
	
	string GetGamemodeString(Game.GamemodeIdxs gamemodeIdx)
	{
		switch (gamemodeIdx)
		{
			case Game.GamemodeIdxs.FreeForAll:
				return "Free For All";
			case Game.GamemodeIdxs.TEAMS:
				return "Team Deathmatch";
			case Game.GamemodeIdxs.CaptureTheFlag:
				return "Capture The Flag";
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
			
			currentLabel.Text = GetGamemodeString((Game.GamemodeIdxs)NormalizeIdx(i + currentIdx + labelGMIdxOffset , contents.Length));
			currentLabel.Size *= Vector2.Down;
		}	
    }


	public void SetGamemode()
	{
		Game.SetGamemode((Game.GamemodeIdxs)currentIdx);
	}
}
