using Godot;
using System;

public partial class PlayerCapsule : MenuItem
{
	[ExportGroup("Nodes")]
	[Export] MainMenuScreenOptions menuScreenOptions;
	public bool isEnabled = false;
	MenuItem currentInteractible;
	MenuSelectPanel menuSelectPanel;

    public override bool OnMoveAction(Vector2 dir)
    {
		if (currentInteractible.OnMoveAction(dir)) return true;
		
		if (menuSelectPanel == null)
		{
			MenuItem[] options = menuScreenOptions.GetMenuItems();
			menuSelectPanel = MenuSelectPanel.GetNewPannel(options);
			options[0].AddChild(menuSelectPanel);
		}
		currentInteractible = menuSelectPanel.MoveGetSelectedNode(dir);
		
		return true;
	}

    public override bool OnPositiveAction()
    {
        return base.OnPositiveAction();
    }

    public override bool OnNegativeAction()
    {
        return base.OnNegativeAction();
    }

}
