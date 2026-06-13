using Godot;
using System;

[Tool]
public partial class MainMenu : CanvasLayer
{
    MainMenuController menuController;
    public enum Screens
	{
		MAINMENU,
		GAMEMODESELECT,
		CHARACTERSELECT,
		TEAMSELECT,
		SETTINGS,
		VIDEOSETTINGS,
		AUDIOSETTINGS,
	}
    public override void _Ready()
    {
        Game.Instance.mainMenu = this;
        menuController = GetNode<MainMenuController>("MainMenuController");	
		# if TOOLS
			if (Engine.IsEditorHint()) return;
		# endif
        Reset();
    }

    public MainMenuController GetController()
    {
        return menuController;
    }

    internal void Reset(Screens screen = Screens.MAINMENU)
    {
        MainMenuScreen initialScreen = GetScreen(screen);
        foreach (Node node in GetChildren())
		{
			if (node is not MainMenuScreen mainMenuScreen) continue;
			mainMenuScreen.Position *= 0;
			if (mainMenuScreen != initialScreen)
				mainMenuScreen.CallDeferred(MainMenuScreen.MethodName.Move, Vector2.Right, false, false, true);
		}
    }
	
    public MainMenuScreen GetScreen(Screens screen)
	{
		switch (screen)
		{
			case Screens.MAINMENU:
				return GetNodeOrNull<MainMenuScreen>("MainMenuMainScreen");
			case Screens.GAMEMODESELECT:
				return GetNodeOrNull<MainMenuScreen>("GamemodeSelectScreen");
			case Screens.CHARACTERSELECT:
				return GetNodeOrNull<MainMenuScreen>("CharacterSelectScreen");
			case Screens.TEAMSELECT:
				return GetNodeOrNull<MainMenuScreen>("TeamSelectScreen");
			case Screens.SETTINGS:
				return GetNodeOrNull<MainMenuScreen>("SettingsScreen");
			case Screens.VIDEOSETTINGS:
				return GetNodeOrNull<MainMenuScreen>("VideoSettingsScreen");
			case Screens.AUDIOSETTINGS:
				return GetNodeOrNull<MainMenuScreen>("AudioSettingsScreen");
			default:
				GD.PrintErr("screen not included in GetScreen(). returning child 0");
				return GetChild<MainMenuScreen>(0);
		}
	}
}
