using Godot;
using System;
using System.ComponentModel;

[Tool]
public partial class MenuItem : Control
{
	[Export] bool Selectable = true;
	[Export] public MenuSelectorHelper panelHelper;
	public bool isActivated = false;
	public Vector2 originalPosition;

    public override void _Ready()
    {
		if (panelHelper != null){
			ChildEnteredTree += OnChildEnteredTree;
		}
		originalPosition = Position;
    }
	public MenuSelectorHelper GetPanelHelper()
	{
		return panelHelper;
	}
	
	void OnChildEnteredTree(Node node)
	{
		GD.Print(Owner.Name);
		GD.Print("child entered");
		GD.Print("is select panel", node is not MenuSelectPanel);
		GD.Print("has panel", panelHelper.HasPanel());

		if (node is not MenuSelectPanel selectPanel) return;
		if (selectPanel.oldParent == this || selectPanel.oldParent == panelHelper) return;

		HandleSelectPanel(selectPanel);
	}

	public virtual void HandleSelectPanel(MenuSelectPanel panel)
	{
		panel.CallDeferred(MethodName.Reparent, panelHelper);
		panelHelper.CallDeferred(MenuSelectorHelper.MethodName.SetPosAndSizeClosed);
	}

	public virtual bool OnInteract()
	{
		return false;
	}
	
	public virtual bool OnAltInteractAction()
	{
		return false;
	}

	public virtual bool OnAccept()
	{
		return false;
	}

	public virtual bool OnNegativeAction()
	{
		return false;
	}

	public virtual bool OnMoveAction(Vector2 dir)
	{
		return false;
	}
}
