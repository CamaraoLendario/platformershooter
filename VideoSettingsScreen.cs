using Godot;
using System;

[Tool]
public partial class VideoSettingsScreen : MainMenuScreen
{
    public override void Back()
    {
        Move(Vector2.Down);
		SettingsScreen settingsScreen = GetParent().GetNode<SettingsScreen>("SettingsScreen");
		settingsScreen.Move(Vector2.Up, true);
		GetNode<MainMenuController>("%MainMenuController").currentScreen = settingsScreen;
    }
}
