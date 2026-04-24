using Godot;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class MainMenuScreen : Control
{
	[Signal] public delegate void EnteredEventHandler();
	[Signal] public delegate void LeftEventHandler();
	[ExportGroup("Menu Selector Panel settings")]
	[Export] bool usesSelectorPanel = true;
	MenuItem[] menuOptions;
	protected MenuSelectPanel menuSelectPanel;
	protected MainMenuController mainMenuController;
	protected MenuItem currentInteractible;
    public override void _Ready()
    {
		if (!usesSelectorPanel) return;
		menuOptions = GetNode<MainMenuScreenOptions>("MenuItemsContainer").GetMenuItems();
		if (menuOptions.Length > 0)
			currentInteractible = menuOptions[0];
		mainMenuController = GetNode<MainMenuController>("%MainMenuController");
		SpawnMenuSelectPanel();
    }

	public virtual bool OnPositiveAction()
	{
		return currentInteractible.OnPositiveAction();
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
		if (currentInteractible.OnMoveAction(dir)) return;
		
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
		GD.PrintErr("Menu Error at ", GetParent().Name, ": ", errorMessage);
	}

	public virtual async void Move(Vector2 dir, bool reverse = false, bool skipAnimation = false)
	{	
		if (!reverse)
			EmitSignal(SignalName.Entered);
		else
			EmitSignal(SignalName.Left);

		if (reverse) await ToSignal(GetTree().CreateTimer(0.1f), Timer.SignalName.Timeout);

		float animationTime = 0.6f;
		if (skipAnimation) animationTime *= 0;
	
		Tween tween = CreateTween();

		MenuItem[] Nodes = GetScreenNodes().ToArray();
		int NodesCount = Nodes.Length;
		Vector2 ScreenSize = new(
			(float) ProjectSettings.GetSetting("display/window/size/viewport_width"),
			(float) ProjectSettings.GetSetting("display/window/size/viewport_height")
		);
		
		float reverseTweenValue = 0;
		if (reverse)
			reverseTweenValue = 1;

		tween.TweenMethod(Callable.From((float tweenedValue) =>{
			for(int i = 0; i < NodesCount; i++)
			{
				MenuItem node;
				if (dir.Y > 0 && !reverse || dir.Y < 0 && reverse) node = Nodes[i];
				else  node = Nodes[NodesCount - 1 - i];
				float tempTweenedValue = Mathf.Clamp((tweenedValue * 1.5f) - ((1-((i + 1)/((float)NodesCount)))*0.5f), 0 ,1);
				tempTweenedValue = (Mathf.Sin((tempTweenedValue-.5f) * 2 * (Mathf.Pi/2)) + 1)/2;
				tempTweenedValue = Mathf.Abs(reverseTweenValue - tempTweenedValue);
				node.Position = node.originalPosition + (dir * tempTweenedValue * ScreenSize);
			}
		}), 0f, 1f, animationTime);
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
			if (child is MenuItem menuItem)
				newMenuItems.Add(menuItem);
			else if (child is MainMenuScreenOptions screenOptions){
				foreach (MenuItem nestedMenuItem in screenOptions.GetMenuItems())
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
}
