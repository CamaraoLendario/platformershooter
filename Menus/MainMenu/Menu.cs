using Godot;
using static SpaceMages.SpaceMagesVars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;

public partial class Menu : MainMenuScreen
{
	int currentButtonIdx;

	bool Play()
	{
		return false;
	}

	bool Settings()
	{
		return false;
	}

	bool Credits()
	{
		return false;
	}
	
	bool Quit()
	{
		GetTree().Quit();
		return true;
	}

    public override void OnMoveAction(Vector2 dir)
    {
		currentButtonIdx = menuSelectPanel.Move(dir);
    }
    public override bool OnPositiveAction()
    {
        switch (currentButtonIdx)
		{
			case 0: return Play();
			case 1: return Settings();
			case 2: return Credits();
			case 3: return Quit();
			default:return false;
		};
    }
}
