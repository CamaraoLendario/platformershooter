using Godot;
using System;

public partial class GamemodeSettings : MainMenuScreen
{
    [Export] GamemodeScroll gamemodeScroll;
    [Export] Control gamemodeScrollPanelHelper;
    [Export] Control currentInteractible;

    public override void OnMoveAction(Vector2 dir)
    {
        if (dir.Y == 0) return;

        if (currentInteractible == gamemodeScroll)
        {
            gamemodeScroll.Scroll((int)dir.Y);   
        }
        else
        {
            currentInteractible = menuSelectPanel.MoveGetNode(dir);
        }
    }

    public override void OnNegativeAction()
    {
        if (currentInteractible == gamemodeScroll)
        {
            currentInteractible = gamemodeScrollPanelHelper;
            gamemodeScroll.Close();
            return;
        }
        
        HideMenu();
        (GetNode("%Menu") as Menu).ShowMenu();
    }
    public override void OnPositiveAction()
    {
        if (currentInteractible == gamemodeScrollPanelHelper)
        {
            currentInteractible = gamemodeScroll;
            gamemodeScroll.Open();
        }
    }
}
