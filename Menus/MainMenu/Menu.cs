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
	PackedScene packedScene;
	int currentButtonIdx;

	void Play()
	{
		HideMenu();
		GamemodeSettings gamemodeSettings = GetNode("%GamemodeSettings") as GamemodeSettings;
		gamemodeSettings.ShowMenu();
	}

	void Settings()
	{
		HideRight();
		SettingsMenu settingsMenu = GetNode("%SettingsMenu") as SettingsMenu;
		settingsMenu.ShowMenu();
	}

	void Credits()
	{
		
	}
	
	void Quit()
	{
		GetTree().Quit();
	}

    public override void OnMoveAction(Vector2 dir)
    {
		currentButtonIdx = menuSelectPanel.Move(dir);
    }
    public override void OnPositiveAction()
    {
        switch (currentButtonIdx)
		{
			case 0:
				Play();
				break;
			case 1:
				Settings();
				break;
			case 2:
				Credits();
				break;
			case 3:
				Quit();
				break;
		};
    }
    public override void OnNegativeAction()
	{
		MainMenuAnimator.Play("MainCardPressedAnyKey", -1, -1);
		mainMenuController.currentScreen = mainMenuController.mainCard;
	}

	public void HideRight()
	{
		SelfAnimator.Play("HideRight");
	}

	public void ShowRight()
	{
		SelfAnimator.Play("ShowFromRight");
		mainMenuController.currentScreen = this;
	}
}
