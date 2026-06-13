using Godot;
using System;

public partial class OverWorld : Node2D
{
	public bool canPause = true;
	World world;
    public override void _Ready()
	{
		world = GetTree().GetFirstNodeInGroup("World") as World;
	}

	public void ClearMap()
	{
		world.ClearMap();
	}
	public World GetWorld()
	{
		return world;
	}
}
