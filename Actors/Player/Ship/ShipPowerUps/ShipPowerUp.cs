using Godot;
using System;
using System.Collections;

public partial class ShipPowerUp : Node2D
{
	public int powerUpID = -1;
    protected ShipPowerUpsHolder holder;
    protected ShipAttack controller;

    public override void _Ready()
    {
        holder = GetParent<ShipPowerUpsHolder>();
        controller = holder.controller;
        Game.Instance.NewRoundStarted += End;
    }

    public virtual void End()
    {
        QueueFree();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Game.Instance.NewRoundStarted -= End;
    }

}
