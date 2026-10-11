using Godot;
using System;

[Tool]
public partial class AudioSettingsScreen : MainMenuScreen
{
    public override bool Back()
    {
        ChangeScreen(
            GetParent().GetNode<SettingsScreen>("SettingsScreen"),
            Vector2.Down, Vector2.Up);
        return true;
    }
}
