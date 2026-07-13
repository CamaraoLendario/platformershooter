using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using static SpaceMages.SpaceMagesVars;


[Tool]
public partial class TeamSelectScreen : MainMenuScreen
{
	[Export] MenuItemsListContainer playersContainer;
	HBoxContainer startGame;
	PackedScene playerIcon = GD.Load<PackedScene>("uid://bm3c06itcq1v7");
	TeamSelectScreenPlayerIcon[] icons = [];
	Tween playerIconTweener;

    public override void _Ready()
	{
		startGame = GetNode<HBoxContainer>("StartGame");
	}

	public void Initialize(PlayerInfo[] playersInfo)
	{
		SpawnPlayerContainers(playersInfo);
	}
	void RemovePlayerContainers()
	{
		foreach (Control Child in icons)
		{
			if (Child is TeamSelectScreenPlayerIcon icon){
				icon.move -= MoveIcon;
			}

			Child.QueueFree();
		} 
		icons = [];
	}
	void SpawnPlayerContainers(PlayerInfo[] playersInfo)
	{
		RemovePlayerContainers();
		icons = new TeamSelectScreenPlayerIcon[playersInfo.Length];
		for (int i = 0; i < playersInfo.Length; i++) {
			PlayerInfo playerInfo = playersInfo[i];
			TeamSelectScreenPlayerIcon newPlayerContainer = playerIcon.Instantiate<TeamSelectScreenPlayerIcon>();
			newPlayerContainer.Setup(playerInfo.Name, playerInfo.inputIdx, playerInfo.colorIdx);
			//newPlayerContainer.Hide();
			icons[i] = newPlayerContainer;
			newPlayerContainer.move += MoveIcon;
		}
		playersContainer.MassAddChildren(icons);
	}
    public override void Move(Vector2 dir, bool reverse = false, bool isReverseOrder = false, bool skipAnimation = false)
    {
        base.Move(dir, reverse, isReverseOrder, skipAnimation);
		if (!reverse){
			EmitSignal(SignalName.Entered, skipAnimation);
			startGame.Hide();
		}
		else
			EmitSignal(SignalName.Left, skipAnimation);

		if (playerIconTweener is not null && playerIconTweener.IsRunning()){
			playerIconTweener.Kill();
		}

		float animationTime = 0.6f;
		if (skipAnimation) animationTime *= 0;
	
		MenuItem[] Nodes = GetPlayerIcons();
		int NodesCount = Nodes.Length;
	
		Vector2 ScreenSize = GetScreenRez();
		
		float reverseTweenValue = 0;
		float delay = 0f;
		if (reverse){
			reverseTweenValue = 1;
			delay = 0.1f;
		}

		playerIconTweener = CreateTween();
		playerIconTweener.TweenMethod(Callable.From((float tweenedValue) =>{
			for(int i = 0; i < NodesCount; i++)
			{
				MenuItem node;
				if ((dir.Y > 0 && !reverse || dir.Y < 0 && reverse) == !isReverseOrder) node = Nodes[i];
				else  node = Nodes[NodesCount - 1 - i];
				float tempTweenedValue = Mathf.Max(0, tweenedValue)/0.6f;
				
				if (reverse){
					tempTweenedValue = Mathf.Max(0, tempTweenedValue);
				}
				tempTweenedValue = Mathf.Clamp((tempTweenedValue * 1.5f) - ((1-((i + 1)/((float)NodesCount)))*0.5f), 0 ,1);
				tempTweenedValue = (Mathf.Sin((tempTweenedValue - .5f) * 2 * (Mathf.Pi/2)) + 1)/2;
				tempTweenedValue = Mathf.Abs(reverseTweenValue - tempTweenedValue);
				node.Position = node.originalPosition + (dir * tempTweenedValue * ScreenSize);
			}
		}), -delay, 0.6, animationTime + delay);
	}
    public override void _Input(InputEvent @event)
    {
		if (GetMenuController().currentScreen != this) return;
        base._Input(@event);
		GD.Print("inputted teamselect");
		if (Input.IsActionJustPressed("MenuAccept") || Input.IsActionJustPressed("MenuAcceptKeyboard")) {
			GD.Print("starging game teams");
			StartGame();
		}
    }

	public void StartGame()
	{
		if (startGame.Visible) {
			TeamsLogic teamsGamemodeLogic = Game.GetGamemodeLogic() as TeamsLogic;
			foreach (TeamSelectScreenPlayerIcon icon in icons) {
				teamsGamemodeLogic.playerTeamByInputIdx.Add(icon.GetInputIdx(), icon.currentTeam);
			}
           	Game.StartGame(Game.GetPlayersInfo(), new MapPlaylist());
		}
	}

    public override bool OnAltInteract()
	{
		RandomizeTeams();
		return true;
	}
	void RandomizeTeams()
	{
		int leftCount = 0;
		int rightCount = 0;
		int maxTeamSize = (int)Math.Ceiling((float)icons.Length/2);
		foreach (TeamSelectScreenPlayerIcon icon in icons)
		{
			if ((GD.RandRange(0, 1) == 0 || leftCount >= maxTeamSize) && rightCount < maxTeamSize)
			{
				icon.MoveRight();
				rightCount ++;
			}
			else
			{
				icon.MoveLeft();
				leftCount++;
			} 
		}
	}

	void MoveIcon(Vector2 dir, TeamSelectScreenPlayerIcon icon)
	{
		if (dir.X == 0) return;
		icon.currentTeam = (int)((dir.X + 1)/2);
		CheckAllChose();
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.InOut);
		Vector2 initialPosition = icon.Position;
		Vector2 finalPosition = (dir * 500f) - (icon.Size * Vector2.Right / 2) + initialPosition * Vector2.Down;
		Vector2 posDiff = finalPosition - initialPosition;
		tween.TweenMethod(Callable.From((float tweenedValue) =>
		{
			icon.Position = initialPosition + (posDiff * tweenedValue);
		}), 0f, 1f, 0.2);

	}
    public override void Back()
	{
		RemovePlayerContainers();
		CharacterSelectScreen characterSelectScreen = GetParent().GetNode<CharacterSelectScreen>("CharacterSelectScreen"); 
		characterSelectScreen.Reconstruct();
		ChangeScreen(characterSelectScreen,
			Vector2.Right, Vector2.Left, false, true
		);
	}
	MenuItem[] GetPlayerIcons()
	{
		List<MenuItem> menuItems = [];
		foreach (Node node in playersContainer.GetChildren()){
			if (node is MenuItem menuItem && !menuItem.IsQueuedForDeletion())
				menuItems.Add(menuItem);
		}
		return menuItems.ToArray();
	}
	void CheckAllChose()
	{
		if (icons.Length == 0){ 
			startGame.Hide();
			return;
		}	
		foreach(TeamSelectScreenPlayerIcon icon in icons)
		{
			if (icon.currentTeam != 1 && icon.currentTeam != 0)
			{
				startGame.Hide();
				return;	
			}
		}
		startGame.Show();
	}
}
