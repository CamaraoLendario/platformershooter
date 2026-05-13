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
    HBoxContainer startGame;

    public override void _Ready()
    {
        base._Ready();
        
        startGame = GetNode<HBoxContainer>("StartGame");
        foreach(PlayerCapsule playerCapsule in GetAllPlayerCapsules())
        {
            playerCapsule.Disabled += OnCapsuleDisabled;
            playerCapsule.Readied += OnCapsuleReady;
            playerCapsule.UnReadied += OnCapsuleUnReady; 
            playerCapsule.characterSelectScreen = this;
        }
    }
    public void Reconstruct(Dictionary<string, int>[] playersInfo = null)
    {
        if (playersInfo == null) playersInfo = Game.Instance.playersInfo;
        PlayerCapsule[] playerCapsules = GetAllPlayerCapsules();
        foreach (PlayerCapsule playerCapsule in playerCapsules)
            playerCapsule.Disable();
        if (playersInfo.Length == 0){
            GD.PrintErr("Reconstruction Failed, No player info avaliable");
            return;
        }
        for(int i = 0; i < playersInfo.Length; i++)
        {
            Dictionary<string, int> playerInfo = playersInfo[i];
            PlayerCapsule playerCapsule = playerCapsules[i];
            
            EnableCapsule(playerCapsule, playerInfo["inputIdx"]);
            playerCapsule.SetPlayerName(playerInfo.Keys.First());
            playerCapsule.SetColor(playerInfo["colorIdx"]);
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

        EnableCapsule(GetFirstDisabledCapsule(), inputIdx);
    }
    void EnableCapsule(PlayerCapsule capsule, int inputIdx)
    {
        for (int i = 0; i < inputIdxs.Length; i++)
        {
            if (inputIdxs[i] == -2){
                inputIdxs[i] = inputIdx;
                break;
            }
        }
        capsule.Enable(inputIdx);
    }
	PlayerCapsule[] GetAllPlayerCapsules() 
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
        bool isEveryoneReady = true;
        
        foreach(PlayerCapsule capsule in GetAllPlayerCapsules())
        {
            if (capsule.isEnabled && !capsule.isReady)
            {
                isEveryoneReady = false;
                if (capsule.colorIdx == playerCapsule.colorIdx)
                    capsule.SetColor(capsule.colorIdx+1);
                return;
            }
        }
        startGame.Visible = isEveryoneReady;
    }
    public override bool OnAccept()
    {
        if (!startGame.Visible) return false;
        TeamSelectScreen teamSelectScreen = GetParent().GetNode<TeamSelectScreen>("TeamSelectScreen");
        teamSelectScreen.Initialize(GetPlayersInfo());
        ChangeScreen(
            teamSelectScreen,
            Vector2.Left, Vector2.Right);
        return true;
    }
    void OnCapsuleUnReady(PlayerCapsule playerCapsule){
        colorUnavaliability[playerCapsule.colorIdx] = false;
        startGame.Visible = false;
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
    Dictionary<string, int>[] GetPlayersInfo()
    {
        Dictionary<string, int>[] playersInfo = [];

        foreach (PlayerCapsule capsule in GetEnabledPlayerCapsules())
        {
            if (!capsule.isReady)
                GD.PrintErr(capsule.GetPlayerName(), " is not ready! getting info anyways");
            
            Dictionary<string, int> playerInfo = new()
            {
                {capsule.GetPlayerName(), 0},
                {"inputIdx", capsule.GetInputIdx()},
                {"colorIdx", capsule.GetColorIdx()}
            };

            playersInfo = playersInfo.Append(playerInfo).ToArray();
        }
        Game.Instance.playersInfo = playersInfo;
        return playersInfo;
    }
}
