using Godot;
using System;

[Tool]
public partial class SettingsScreen : MainMenuScreen
{
    public override bool OnInteract()
    {
		switch (menuSelectPanel.GetCurrentNodeIdx())
		{
			case 0:
				VideoSettings();
				break;
			case 1:
				AudioSettings();
				break;
			case 2:
				Accessibility();
				break;
				
		}
		return false;
    }
	void VideoSettings()
	{
		Move(Vector2.Up);
		VideoSettingsScreen videoSettingsScreen = GetParent().GetNode<VideoSettingsScreen>("VideoSettingsScreen");
		videoSettingsScreen.Move(Vector2.Down, true);
		GetNode<MainMenuController>("%MainMenuController").currentScreen = videoSettingsScreen;
	}
	void AudioSettings()
    {
		Move(Vector2.Up);
		AudioSettingsScreen audioSettingsScreen = GetParent().GetNode<AudioSettingsScreen>("AudioSettingsScreen");
		audioSettingsScreen.Move(Vector2.Down, true);
		GetNode<MainMenuController>("%MainMenuController").currentScreen = audioSettingsScreen;
    }
	void Accessibility(){}

    public override void Back()
    {
        Move(Vector2.Left);
		MainMenuMainScreen mainMenuMain = GetParent().GetNode<MainMenuMainScreen>("MainMenuMainScreen");
		mainMenuMain.Move(Vector2.Right, true);
		GetNode<MainMenuController>("%MainMenuController").currentScreen = mainMenuMain;
    }
}
