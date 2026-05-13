using Godot;
using System;

[Tool]
public partial class AudioSettingsScreen : MainMenuScreen
{
    public override void Back()
    {
        ChangeScreen(
            GetParent().GetNode<SettingsScreen>("SettingsScreen"),
            Vector2.Down, Vector2.Up);
    }
}
