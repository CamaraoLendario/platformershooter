using Godot;
using System;

public partial class FrozenDDR : Node2D
{
	[Export] DDRInputsParent DDRparent;
	Player frozenPlayer; 
    public override void _Ready()
    {
		frozenPlayer = GetParent<Player>();
		DDRparent.frozenPlayer = frozenPlayer;
        DDRparent.ended += () =>
		{
			frozenPlayer.effectHandler.UnFreeze();
		};
	}

}
