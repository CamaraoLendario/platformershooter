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
		ChangeScreen(
            GetParent().GetNode<VideoSettingsScreen>("VideoSettingsScreen"),
            Vector2.Up, Vector2.Down);
	}
	void AudioSettings()
    {
		ChangeScreen(
            GetParent().GetNode<AudioSettingsScreen>("AudioSettingsScreen"),
            Vector2.Up, Vector2.Down);
		Move(Vector2.Up);
    }
	void Accessibility(){}

    public override void Back()
    {
		ChangeScreen(
            GetParent().GetNode<MainMenuMainScreen>("MainMenuMainScreen"),
            Vector2.Left, Vector2.Right);
    }
}
