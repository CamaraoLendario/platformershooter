using Godot;
using System;

public partial class ShipPowerupAnimationPlayer : AnimationPlayer
{
    public override void _Ready()
    {
        AnimationFinished += OnAnimationFinished;
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "SpawnSpace")
		{
			Play("SpaceRotation");
		}
    }
}
