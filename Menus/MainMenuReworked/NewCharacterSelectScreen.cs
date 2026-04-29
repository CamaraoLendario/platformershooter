using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class NewCharacterSelectScreen : MainMenuScreen
{
    int[] inputIdxs = [
        -2, -2, -2, -2, -2, -2
    ];
    bool[] colorUnavaliability = new bool[teamColors.Length];

    public override void _Ready()
    {
        base._Ready();
        foreach(PlayerCapsule playerCapsule in GetPlayerCapsules())
        {
            playerCapsule.Disabled += OnCapsuleDisabled;
            playerCapsule.Readied += OnCapsuleReady;
            playerCapsule.UnReadied += OnCapsuleUnReady; 
            playerCapsule.characterSelectScreen = this;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsReleased() || @event is InputEventMouse || GetMenuController().currentScreen != this) return;

        int inputIdx = @event.Device;
        if (@event is InputEventKey)
            inputIdx = -1;

        if (inputIdxs.Contains(inputIdx)){
            GD.PrintErr($"Input index already registered! ({inputIdx})");
            return;
        }
        if (!inputIdxs.Contains(-2))
        {
            GD.PrintErr("Max player count reached!");
            return;
        }

        for (int i = 0; i < 6; i++)
        {
            if (inputIdxs[i] == -2){
                inputIdxs[i] = inputIdx;
            }
        }

        GetFirstDisabledCapsule().Enable(inputIdx);
    }
    PlayerCapsule GetFirstDisabledCapsule()
    {
        PlayerCapsule capsule = null;
        foreach (MenuItem menuItem in menuOptions)
        {
            if (menuItem is not PlayerCapsule playerCapsule || playerCapsule.isEnabled) continue;
            capsule = playerCapsule;
        }
        if (capsule == null)
        {
            GD.PrintErr("All capsules are occupied!");
        }
        return capsule;
    }
	PlayerCapsule[] GetPlayerCapsules()
    {
        PlayerCapsule[] capsules = [];
        foreach (MenuItem menuItem in menuOptions)
        {
            if (menuItem is not PlayerCapsule playerCapsule) continue;
            capsules = capsules.Append(playerCapsule).ToArray();
        }
        return capsules;
    }
    PlayerCapsule[] GetEnabledPlayerCapsules()
    {
        PlayerCapsule[] capsules = [];
        foreach (MenuItem menuItem in menuOptions)
        {
            if (menuItem is not PlayerCapsule playerCapsule || !playerCapsule.isEnabled) continue;
            capsules = (PlayerCapsule[])capsules.Append(playerCapsule);
        }
        return capsules;
    }
    void OnCapsuleDisabled(PlayerCapsule playerCapsule){
        for(int i = 0; i < inputIdxs.Length; i++){
            if (inputIdxs[i] == playerCapsule.menuInput.inputIdx){
                inputIdxs[i] = -2;
                return;
            }
        }
    }
    void OnCapsuleReady(PlayerCapsule playerCapsule){
        colorUnavaliability[playerCapsule.colorIdx] = true;
    }
    void OnCapsuleUnReady(PlayerCapsule playerCapsule){
        colorUnavaliability[playerCapsule.colorIdx] = false;
    }
    public bool IsColorAvaliable(int colorIdx){
        return !colorUnavaliability[colorIdx];
    }
}
