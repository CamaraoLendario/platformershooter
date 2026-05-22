using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
		screenMoveTween = CreateTween();

		float animationTime = 0.6f;
		if (skipAnimation) animationTime *= 0;
	
		MenuItem[] Nodes = GetScreenNodes().ToArray();
		float delay = reverse ? 0.1f : 0f;
		
		ScreenAnimateNodes(screenMoveTween, Nodes, dir, animationTime, reverse, isReverseOrder, delay);
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
