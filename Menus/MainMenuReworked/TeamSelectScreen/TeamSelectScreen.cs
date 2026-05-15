using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static SpaceMages.SpaceMagesVars;


[Tool]
public partial class TeamSelectScreen : MainMenuScreen
{
	[Export] MenuItemsListContainer playersContainer;
	PackedScene playerIcon = GD.Load<PackedScene>("uid://bm3c06itcq1v7");
	Control[] Containers = [];
	Tween playerIconTweener;

	public void Initialize(Dictionary<string, int>[] playersInfo)
	{
		RemovePlayerContainers();
		SpawnPlayerContainers(playersInfo);
	}
	void RemovePlayerContainers()
	{
		foreach (Control Child in Containers)
		{
			if (Child is TeamSelectScreenPlayerIcon icon){
				icon.moveLeft -= MoveIcon;
				icon.moveRight -= MoveIcon;
			}

			Child.QueueFree();
		} 
		Containers = [];
	}
	void SpawnPlayerContainers(Dictionary<string, int>[] playersInfo)
	{
		foreach (Dictionary<string, int> playerInfo in playersInfo){
			TeamSelectScreenPlayerIcon newPlayerContainer = playerIcon.Instantiate<TeamSelectScreenPlayerIcon>();
			newPlayerContainer.Setup(playerInfo.Keys.First(), playerInfo["inputIdx"], playerInfo["colorIdx"]);
			//newPlayerContainer.Hide();
			Containers = Containers.Append(newPlayerContainer).ToArray();
			newPlayerContainer.moveLeft += MoveIcon;
			newPlayerContainer.moveRight += MoveIcon;
		}
		playersContainer.MassAddChildren(Containers);
	}
    public override void Move(Vector2 dir, bool reverse = false, bool isReverseOrder = false, bool skipAnimation = false)
    {
        base.Move(dir, reverse, isReverseOrder, skipAnimation);
		if (!reverse)
			EmitSignal(SignalName.Entered);
		else
			EmitSignal(SignalName.Left);

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
				tempTweenedValue = (Mathf.Sin((tempTweenedValue-.5f) * 2 * (Mathf.Pi/2)) + 1)/2;
				tempTweenedValue = Mathf.Abs(reverseTweenValue - tempTweenedValue);
				node.Position = node.originalPosition + (dir * tempTweenedValue * ScreenSize);
			}
		}), -delay, 0.6, animationTime + delay);
	}
	void MoveIcon(Vector2 dir, TeamSelectScreenPlayerIcon icon)
	{
		if (dir.X > 0) 
			Game.Instance.playerTeams.Add(icon.GetInputIdx(), (int)TeamIdxs.BLUE);
		else 
			Game.Instance.playerTeams.Add(icon.GetInputIdx(), (int)TeamIdxs.RED);

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
		CharacterSelectScreen characterSelectScreen = GetParent().GetNode<CharacterSelectScreen>("NewCharacterSelectScreen"); 
		characterSelectScreen.Reconstruct();
		ChangeScreen(characterSelectScreen,
			Vector2.Right, Vector2.Left, false, true
		);
	}

	MenuItem[] GetPlayerIcons()
	{
		MenuItem[] menuItems = [];
		foreach (Node node in playersContainer.GetChildren()){
			if (node is MenuItem menuItem)
				menuItems = menuItems.Append(menuItem).ToArray();
		}
		return menuItems;
	}
}
