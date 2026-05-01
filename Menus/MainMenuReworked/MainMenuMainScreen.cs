using Godot;
using System;
using System.Linq.Expressions;

[Tool]
public partial class MainMenuMainScreen : MainMenuScreen
{
    public override bool OnInteract()
    {
		switch (menuSelectPanel.GetCurrentNodeIdx())
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
				
		}
		return false;
    }
	void Play()
	{
		Move(Vector2.Left);
		GamemodeSelectScreen gamemodeSelectScreen = GetParent().GetNode<GamemodeSelectScreen>("GamemodeSelectScreen");
		gamemodeSelectScreen.Move(Vector2.Right, true);
		GetMenuController().currentScreen = gamemodeSelectScreen;
	}
	void Settings()
	{
		Move(Vector2.Right);
		SettingsScreen settingsScreen = GetParent().GetNode<SettingsScreen>("SettingsScreen");
		settingsScreen.Move(Vector2.Left, true);
		GetMenuController().currentScreen = settingsScreen;
	}
	void Credits()
	{
		
	}
	void Quit()
	{
		if(Engine.IsEditorHint()) return;
		GetTree().Quit();
	}

    public override void Back()
	{
		ChangeScreen(
            GetParent().GetNode<MainThemeScreen>("MainThemeScreen"),
            Vector2.Down, Vector2.Down);
	}
}
