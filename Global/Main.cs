using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial class Main : Node
{
    public ConstantOverlay constantOverlay;
    OverWorld overWorld;
    MainMenu mainMenu;
    public override void _Ready()
    {
        Game.Instance.main = this;
        constantOverlay = GetNode<ConstantOverlay>("%ConstantOverlay");
        overWorld = GetNode<OverWorld>("%OverWorld");
    }

    public async void StartGame()
	{
        mainMenu = Game.Instance.mainMenu;
        mainMenu.Reset();
        mainMenu.Hide();
        overWorld.Show();
        CallDeferred(MethodName.EmitGameStart);
        //constantOverlay.FadeIn();
        //constantOverlay.CallDeferredThreadGroup(ConstantOverlay.MethodName.FadeIn);
    }
    void EmitGameStart()
    {
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.StartGame);
        
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameStarted);
    }
    public void BackToCharacterSelectScreen()
    {
        Game.UnPauseGame();
        overWorld.ClearMap();
        Game.ClearPlayerInfo();
        InputGenerator.Instance.ClearExtraInputs(false);
        mainMenu.Show();
        MainMenuScreen currentScreen = mainMenu.GetController().SetCurrentScreen(MainMenu.Screens.CHARACTERSELECT);
        SignalBus.Instance.EmitSignal(SignalBus.SignalName.GameExited, currentScreen);
    }
    public MainMenu GetMainMenu()
    {
        return mainMenu;
    }
}
