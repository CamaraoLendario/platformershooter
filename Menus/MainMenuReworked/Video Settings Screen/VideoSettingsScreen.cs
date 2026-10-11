using Godot;
using System;

[Tool]
public partial class VideoSettingsScreen : MainMenuScreen
{
    HorizontalTextAndScroll WindowSettings;
    HorizontalTextAndText Vsync;
    HorizontalTextAndText doLimitFPS;
    HorizontalTextAndScroll FPSLimit;
    int fpsLimit = 60;
    public override void _Ready()
    {
        base._Ready();

        MenuItemsContainer optionsContainer = GetNode<MenuItemsContainer>("MenuItemsContainer");
        WindowSettings = optionsContainer.GetNode<HorizontalTextAndScroll>("Window Settings");
        WindowSettings.settingChanged += (int newSetting) => {OnWindowSettingsSettingChanged(newSetting);};
        Vsync = optionsContainer.GetNode<HorizontalTextAndText>("Vsync");
        Vsync.settingChanged += (int newSetting) => {OnVsyncSettingChanged(newSetting);};
        doLimitFPS = optionsContainer.GetNode<HorizontalTextAndText>("do Limit FPS");
        doLimitFPS.settingChanged += (int newSetting) => {OnDoLimitFPSSettingChanged(newSetting);};
        FPSLimit = optionsContainer.GetNode<HorizontalTextAndScroll>("FPS Limit");
        FPSLimit.settingChanged += (int newSetting) => {OnFPSLimitSettingChanged(newSetting);};

    }

    public override bool Back()
    {
        ChangeScreen(
            GetParent().GetNode<SettingsScreen>("SettingsScreen"),
            Vector2.Down, Vector2.Up);
        return true;
    }
    void OnWindowSettingsSettingChanged(int newSetting){
        switch (newSetting) {
            case 0:
                // fullscreen
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                GetWindow().Borderless = false;
                return;
            case 1:
                // windowed
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                GetWindow().Borderless = false;
                return;
            case 2:
                // borderlesswindowed
                GetWindow().Borderless = true;
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                return;
            default:
                return;
        }
    }
    void OnVsyncSettingChanged(int newSetting){
        switch (newSetting) {
            case 0:
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
                return;
            case 1:
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
                return;
            default:
                return;
        }
    }
    void OnDoLimitFPSSettingChanged(int newSetting){
        switch (newSetting) {
            case 0:
                Engine.MaxFps = fpsLimit;
                return;
            case 1:
                Engine.MaxFps = 0;
                return;
            default:
                return;
        }
    }
    void OnFPSLimitSettingChanged(int newSetting){
        switch (newSetting) {
            case 0:
                fpsLimit = 120;
                break;
            case 1:
                fpsLimit = 144;
                break;
            case 2:
                fpsLimit = 200;
                break;
            case 3:
                fpsLimit = 240;
                break;
            case 4:
                fpsLimit = 30;
                break;
            case 5:
                fpsLimit = 50;
                break;
            case 6:
                fpsLimit = 60;
                break;
            case 7:
                fpsLimit = 75;
                break;
            case 8:
                fpsLimit = 100;
                break;
            default:
                return;
        }
    
        if (Engine.MaxFps != 0)
            Engine.MaxFps = fpsLimit;
    }
}
