using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

public partial class PilotArea : Node2D
{
	List<(Godot.Vector2 TL, Godot.Vector2 BR)> areas = [];

    public override void _Ready()
	{
		foreach (Node node in GetChildren())
		{
			if (node is CollisionShape2D)
			{
				CollisionShape2D shape = node as CollisionShape2D;
				Vector2 halfSize = (shape.Shape as RectangleShape2D).Size / 2;
				areas.Add((shape.GlobalPosition - halfSize, shape.GlobalPosition + halfSize));
			}
		}
    }

	public override void _PhysicsProcess(double delta)
	{
		foreach (Player player in Game.Instance.playerNodesByColor.Values)
		{
			if (!player.IsDead)
				player.IsInPilotArea = IsInPilotArea(player.GlobalPosition);
		}

		foreach (LinearProjectile bullet in Game.Instance.BulletsNodes)
        {
			if (bullet.isInPilotArea != IsInPilotArea(bullet.GlobalPosition))
            {
				bullet.QueueFree();
            }
        }
	}
	
	public bool IsInPilotArea(Vector2 playerPos)
	{
		foreach ((Vector2 TL, Vector2 BR) area in areas)
        {
			if (playerPos.X < area.BR.X && playerPos.X > area.TL.X && playerPos.Y < area.BR.Y && playerPos.Y > area.TL.Y)
            {
				return true;
            }
        }
		return false;
	}
}
