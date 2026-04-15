using Godot;
using System;

public partial class MainMenuScreen : Control
{
	[Export] protected AnimationPlayer MainMenuAnimator;
	[Export] protected AnimationPlayer SelfAnimator;
	[ExportGroup("Menu Selector Panel settings")]
	[Export] bool usesSelectorPanel = false;
	[Export] Control[] menuOptions;
	protected MenuSelectPanel menuSelectPanel;
	protected MainMenuController mainMenuController;
    public override void _Ready()
    {
        mainMenuController = GetNode<MainMenuController>("%MainMenuController");
		if (usesSelectorPanel)
		{
			SpawnMenuSelectPanel();
		}
    }

	public virtual void OnPositiveAction()
	{
	}
	
	public virtual void OnNegativeAction()
	{
	}

	public virtual void OnMoveAction(Vector2 dir)
	{
		
	}
	
    public void ShowMenu()
    {
        SelfAnimator.Play("Show");
		mainMenuController.currentScreen = this;
    }
	public virtual void HideMenu()
    {
        SelfAnimator.Play("Hide");
    }

	protected void SpawnMenuSelectPanel()
	{
		menuSelectPanel = MenuSelectPanel.GetNewPannel(menuOptions);
		menuOptions[0].AddChild(menuSelectPanel);
	}
}
