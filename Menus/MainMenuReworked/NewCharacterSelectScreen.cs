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

    public override void _UnhandledInput(InputEvent @event)
    {        
        if (@event.IsReleased() || @event is InputEventMouse || GetMenuController().currentScreen != this) return;

        int inputIdx = @event.Device;
        if (@event is InputEventKey)
            inputIdx = -1;

        if (inputIdxs.Contains(inputIdx)){
            return;
        }

        if (Input.IsActionJustPressed("MenuBack") || Input.IsActionJustPressed("MenuBackKeyboard")){
            Back();
            return;
        }

        if (!inputIdxs.Contains(-2))
        {
            GD.PrintErr("Max player count reached!");
            return;
        }

        for (int i = 0; i < inputIdxs.Length; i++)
        {
            if (inputIdxs[i] == -2){
                inputIdxs[i] = inputIdx;
                break;
            }
        }

        GetFirstDisabledCapsule().Enable(inputIdx);
    }
    PlayerCapsule GetFirstDisabledCapsule()
    {
        PlayerCapsule capsule = null;
        foreach (MenuItem menuItem in GetOptions())
        {
            if (menuItem is not PlayerCapsule playerCapsule || playerCapsule.isEnabled) continue;
            capsule = playerCapsule;
            break;
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
        foreach (MenuItem menuItem in GetOptions())
        {
            if (menuItem is not PlayerCapsule playerCapsule) continue;
            capsules = capsules.Append(playerCapsule).ToArray();
        }
        return capsules;
    }
    PlayerCapsule[] GetEnabledPlayerCapsules()
    {
        PlayerCapsule[] capsules = [];
        foreach (MenuItem menuItem in GetOptions())
        {
            if (menuItem is not PlayerCapsule playerCapsule || !playerCapsule.isEnabled) continue;
            capsules = capsules.Append(playerCapsule).ToArray();
        }
        return capsules;
    }
    void OnCapsuleDisabled(PlayerCapsule capsule){
        GD.Print(capsule.Name ," was disabled!!");
        for(int i = 0; i < inputIdxs.Length; i++){
            if (inputIdxs[i] == capsule.GetInputIdx()){
                inputIdxs[i] = -2;
                capsule.MoveToFront();
                return;
            }
        }
    }
    void OnCapsuleReady(PlayerCapsule playerCapsule){
        colorUnavaliability[playerCapsule.colorIdx] = true;
        foreach(PlayerCapsule capsule in GetPlayerCapsules())
        {
            if (capsule.isEnabled && !capsule.isReady && capsule.colorIdx == playerCapsule.colorIdx)
            {
                capsule.SetColor(capsule.colorIdx+1);
            }
        }
    }
    void OnCapsuleUnReady(PlayerCapsule playerCapsule){
        colorUnavaliability[playerCapsule.colorIdx] = false;
    }
    public bool IsColorAvaliable(int colorIdx){
        return !colorUnavaliability[colorIdx];
    }

    public override bool OnNegativeAction()
    {
        return false;
    }
    public override void OnMoveAction(Vector2 dir)
    {
        return;
    }

    public override void Back()
    {
        foreach(PlayerCapsule capsule in GetEnabledPlayerCapsules())
            capsule.Disable();

        ChangeScreen(
            GetParent().GetNode<MainMenuScreen>("GamemodeSelectScreen"),
            Vector2.Right, Vector2.Left, true);
    }
}
