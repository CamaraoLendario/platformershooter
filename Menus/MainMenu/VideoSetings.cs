using Godot;
using System;

public partial class VideoSetings : MainMenuScreen
{
    public override void _Ready()
    {
        base._Ready();
	}

    public override void OnMoveAction(Vector2 dir)
    {
        if (!currentInteractible.OnMoveAction(dir))
		{
			base.OnMoveAction(dir);
		}
    }
}
