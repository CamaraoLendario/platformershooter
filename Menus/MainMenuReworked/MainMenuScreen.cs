using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using static SpaceMages.SpaceMagesVars;

[Tool]
public partial class MainMenuScreen : Control
{
	[Signal] public delegate void EnteredEventHandler();
	[Signal] public delegate void LeftEventHandler();
	[ExportGroup("Menu Selector Panel settings")]
	[Export] bool usesSelectorPanel = true;
	protected MenuItem[] menuOptions;
	protected MenuSelectPanel menuSelectPanel;
	protected MainMenuController mainMenuController;
	protected MenuItem currentInteractible;
	protected Tween screenMoveTween;
    public override void _Ready()
    {
		if (!usesSelectorPanel) return;
		menuOptions = GetNode<MenuItemsListContainer>("MenuItemsContainer").GetMenuItems();
		if (menuOptions.Length > 0)
			currentInteractible = menuOptions[0];
		mainMenuController = GetNode<MainMenuController>("%MainMenuController");
		SpawnMenuSelectPanel();
    }

	public virtual bool OnInteract()
	{
		if (currentInteractible != null)
			return currentInteractible.OnInteract();
		else return false;
	}

	public virtual bool OnAltInteract()
	{
		if (currentInteractible != null)
			return currentInteractible.OnAltInteract();
		else return false;
	}

	public virtual bool OnAccept()
	{
		if (currentInteractible != null)
			return currentInteractible.OnAccept();
		else return false;
	}
		
	public virtual bool OnNegativeAction()
	{
		if (currentInteractible == null)
		{
			Back();
			return false;
		}
		bool result = currentInteractible.OnNegativeAction();
		if (!result)
			Back();
		return result;
	}

	public virtual void OnMoveAction(Vector2 dir)
	{
		if (currentInteractible != null && currentInteractible.OnMoveAction(dir)) return;
		
		if (usesSelectorPanel)
		{
			if (menuSelectPanel == null)
			{
				MenuError("Uses panel but has no panel");
				return;
			}
			currentInteractible = menuSelectPanel.MoveGetSelectedNode(dir);
		}
		else		
			MenuError("Cannot move without a panel");
	}

	public virtual void Back()
	{
		MenuError("Back function not set up");
	}
	protected void SpawnMenuSelectPanel()
	{
		menuSelectPanel = MenuSelectPanel.GetNewPannel(menuOptions);
		menuOptions[0].AddChild(menuSelectPanel);
	}

	protected void MenuError(string errorMessage)
	{
		GD.PrintErr("Menu Error at ", Name, ": ", errorMessage);
	}

	public virtual async void Move(Vector2 dir, bool reverse = false, bool isReverseOrder = false, bool skipAnimation = false)
	{	
		if (!reverse)
			EmitSignal(SignalName.Entered);
		else
			EmitSignal(SignalName.Left);

		if (screenMoveTween is not null && screenMoveTween.IsRunning()){
			screenMoveTween.Kill();
		}

		float animationTime = 0.6f;
		if (skipAnimation) animationTime *= 0;
	
		MenuItem[] Nodes = GetScreenNodes().ToArray();
		int NodesCount = Nodes.Length;
	
		Vector2 ScreenSize = GetScreenRez();
		
		float reverseTweenValue = 0;
		float delay = 0f;
		if (reverse){
			reverseTweenValue = 1;
			delay = 0.1f;
		}

		screenMoveTween = CreateTween();
		screenMoveTween.TweenMethod(Callable.From((float tweenedValue) =>{
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

	protected void ChangeScreen(MainMenuScreen screenTo, Vector2 mainDir, Vector2 screenToDir, bool mainIsReverseOrder = false, bool screenToIsReverseOrder = false)
	{
		MainMenuScreen mainMenuScreen = screenTo;
		GetNode<MainMenuController>("%MainMenuController").currentScreen = mainMenuScreen;
		Move(mainDir, false, mainIsReverseOrder);
		mainMenuScreen.Move(screenToDir, true, screenToIsReverseOrder);
	}

	List<MenuItem> GetScreenNodes()
	{
		return GetScreenNodes(this);
	}
	List<MenuItem> GetScreenNodes(MainMenuScreen parent)
	{
		List<MenuItem> newMenuItems = [];

		foreach(Node child in GetChildren())
		{
			if (child is MenuItem menuItem && !menuItem.IsQueuedForDeletion())
				newMenuItems.Add(menuItem);
			else if (child is MenuItemsListContainer screenOptions){
				foreach (MenuItem nestedMenuItem in screenOptions.GetMenuItems())
					if (!nestedMenuItem.IsQueuedForDeletion())
						newMenuItems.Add(nestedMenuItem);
			}	
		}

		return newMenuItems;
	}

	protected MainMenuController GetMenuController()
	{
		if (mainMenuController == null)
		{		
			mainMenuController = GetNode<MainMenuController>("%MainMenuController");
		}

		return mainMenuController;
	}

	protected MenuItem[] GetOptions()
    {
        menuOptions = GetNode<MenuItemsListContainer>("MenuItemsContainer").GetMenuItems();
		return menuOptions;
    }
}
