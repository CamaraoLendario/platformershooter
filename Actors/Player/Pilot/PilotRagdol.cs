using Godot;
using System;

public partial class PilotRagdol : RigidBody2D
{
	Map currentMap;
	public bool IsInPilotArea
	{
		get
		{
			return isInPilotArea;
		}
		set
		{
			isInPilotArea = value;
			if (value)
			{
				GravityScale = 1;
			}
            else
            {
                GravityScale = 0;
            }
		}
	}
	bool isInPilotArea = true;

	public override void _Ready()
	{
		currentMap = Game.Instance.world.currentMap;

		GetChild<Sprite2D>(0).Rotation = LinearVelocity.Angle() + Mathf.Pi / 2;
		Game.Instance.NewRoundStarted += End;
	}

	void End()
	{
		CallDeferred(MethodName.QueueFree);
	}

	public override void _ExitTree()
	{
		Game.Instance.NewRoundStarted -= End;
		base._ExitTree();
	}

    public override void _PhysicsProcess(double delta)
    {
		IsInPilotArea = currentMap.IsPositionInPilotArea(Position);
    }
}
