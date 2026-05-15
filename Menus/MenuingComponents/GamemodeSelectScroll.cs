using Godot;
using System;

[Tool]
public partial class GamemodeSelectScroll : MenuScroll
{
    public override void _Ready()
    {
		contents = [
			GetGamemodeString(Game.GamemodeIdxs.FreeForAll),
			GetGamemodeString(Game.GamemodeIdxs.TEAMS),
			GetGamemodeString(Game.GamemodeIdxs.CaptureTheFlag)
		];
        base._Ready();
    }
	
	string GetGamemodeString(Game.GamemodeIdxs gamemodeIdx)
	{
		switch (gamemodeIdx)
		{
			case Game.GamemodeIdxs.FreeForAll:
				return "Free For All";
			case Game.GamemodeIdxs.TEAMS:
				return "Teams";
			case Game.GamemodeIdxs.CaptureTheFlag:
				return "Capture The Flag";
		}
		return "";
	}
	Game.GamemodeIdxs GetGamemodeIdx (int idx)
	{
		switch (idx)
		{
			case 0:
				return Game.GamemodeIdxs.FreeForAll;
			case 1:
				return Game.GamemodeIdxs.TEAMS;
			case 2:
				return Game.GamemodeIdxs.CaptureTheFlag;
			default:
				GD.PrintErr("index given is not within the scope (0-2) returning freeforall");
				return Game.GamemodeIdxs.FreeForAll;

		}
	}

	public void SetGamemode()
	{
		Game.SetGamemode(GetGamemodeIdx(currentIdx));
	}
}
