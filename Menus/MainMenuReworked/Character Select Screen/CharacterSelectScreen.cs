using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class CharacterSelectScreen : MainMenuScreen
{
    int[] inputIdxs = [
        -2, -2, -2, -2, -2, -2
    ];
    bool[] colorUnavaliability = new bool[teamColors.Length];
    HBoxContainer startGame;
    HoldBackProgressBar holdBackProgressBar;

    public override void _Ready()
    {
        base._Ready();
        
        Entered += (bool skipAnimation) => {EnableInput();};
        startGame = GetNode<HBoxContainer>("StartGame");
        foreach(PlayerCapsule playerCapsule in GetAllPlayerCapsules())
        {
            playerCapsule.Disabled += OnCapsuleDisabled;
            playerCapsule.Readied += OnCapsuleReady;
            playerCapsule.UnReadied += OnCapsuleUnReady; 
            playerCapsule.characterSelectScreen = this;
        }
        if (GetMenuController().currentScreen == this && Game.GetPlayersInfo().Length > 0) {
            Reconstruct(Game.GetPlayersInfo());
        }
        holdBackProgressBar = GetNode<HoldBackProgressBar>("%HoldBackProgressBar");
        holdBackProgressBar.Success += Back;
    }
    public void Reconstruct(PlayerInfo[] playersInfo = null)
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
            PlayerInfo playerInfo = playersInfo[i];
            PlayerCapsule playerCapsule = playerCapsules[i];
            
            EnableCapsule(playerCapsule, playerInfo.inputIdx);
            playerCapsule.SetPlayerName(playerInfo.Name);
            playerCapsule.SetColor(playerInfo.colorIdx);
        }
    }

    public override void _Input(InputEvent @event)
    {        
        if (@event is InputEventMouse || GetMenuController().currentScreen != this) return;

        int inputIdx = @event.Device;
        if (@event is InputEventKey)
            inputIdx = -1;

        if (Input.IsActionPressed("MenuBack") || Input.IsActionPressed("MenuBackKeyboard"))
            holdBackProgressBar.isBeingHeld = true;
        else
            holdBackProgressBar.isBeingHeld = false;

        if (inputIdxs.Contains(inputIdx)){
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
        startGame.Visible = false;
    }
	PlayerCapsule[] GetAllPlayerCapsules() 
    {
        List<PlayerCapsule> capsules = [];
        foreach (MenuItem menuItem in GetOptions())
        {
            if (menuItem is not PlayerCapsule playerCapsule) continue;
            capsules.Add(playerCapsule);
        }
        return capsules.ToArray();
    }
    PlayerCapsule[] GetEnabledPlayerCapsules()
    {
        List<PlayerCapsule> capsules = [];
        foreach (MenuItem menuItem in GetOptions())
        {
            if (menuItem is not PlayerCapsule playerCapsule || !playerCapsule.isEnabled) continue;
            capsules.Add(playerCapsule);
        }
        return capsules.ToArray();
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
        startGame.Hide();
        if (Game.GetGamemodeLogic().isTeamed)
        {
            TeamSelectScreen teamSelectScreen = GetParent().GetNode<TeamSelectScreen>("TeamSelectScreen");
            teamSelectScreen.Initialize(GetPlayersInfo());
            ChangeScreen(
                teamSelectScreen,
                Vector2.Left, Vector2.Right);
        }
        else
        {
            DisableInput();
            Game.StartGame(GetPlayersInfo(), new MapPlaylist());
        }
        foreach (PlayerCapsule playerCapsule in GetEnabledPlayerCapsules())
        {
            if (!playerCapsule.UnReady()) {
                GD.PrintErr($"unreadying unsuccessful: {playerCapsule.Name}, {playerCapsule}");
            }
        }
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

    public override void Back()
    {
        DisableInput();
        // foreach(PlayerCapsule capsule in GetEnabledPlayerCapsules())
        //     capsule.Disable();

        ChangeScreen(
            GetParent().GetNode<MainMenuScreen>("GamemodeSelectScreen"),
            Vector2.Right, Vector2.Left, true);
    }
    PlayerInfo[] GetPlayersInfo()
    {
        PlayerCapsule[] capsules = GetEnabledPlayerCapsules();
        PlayerInfo[] playersInfo = new PlayerInfo[capsules.Length];

        for (int i = 0; i < capsules.Length; i++)
        {
            PlayerCapsule capsule = capsules[i];
            
            if (!capsule.isReady)
                GD.PrintErr(capsule.GetPlayerName(), " is not ready! getting info anyways");
            
            PlayerInfo playerInfo = new()
            {
                Name = capsule.GetPlayerName(),
                inputIdx = capsule.GetInputIdx(),
                colorIdx =  capsule.GetColorIdx()
            };

            playersInfo[i] = playerInfo;
        }
        Game.Instance.playersInfo = playersInfo;
        return playersInfo;
    }
    public override void Move(Vector2 dir, bool reverse = false, bool isReverseOrder = false, bool skipAnimation = false)
    {
        
        if (reverse){
            Label startGameLabel = startGame.GetNode<Label>("StartGameLabel");
            if (Game.GetGamemodeLogic() != null)
            if (Game.GetGamemodeLogic().isTeamed)
                startGameLabel.Text = "Select Teams";
            else
                startGameLabel.Text = "Start Game";
        }
        base.Move(dir, reverse, isReverseOrder, skipAnimation);
    }
    void EnableInput()
    {
        foreach(PlayerCapsule capsule in GetAllPlayerCapsules())
            capsule.inputNode.inputEnabled = true;
    }
    void DisableInput()
    {
        foreach(PlayerCapsule capsule in GetAllPlayerCapsules())
            capsule.inputNode.inputEnabled = false;
    }
}
