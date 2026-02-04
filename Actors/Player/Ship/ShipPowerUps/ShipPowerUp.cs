using Godot;
using System;
using System.Collections;

public partial class ShipPowerUp : Node2D
{
	protected ShipPowerUpsHolder holder;
    protected ShipAttack controller;
    public override void _Ready()
    {
        holder = GetParent<ShipPowerUpsHolder>();
        controller = holder.controller;
    }

    protected virtual void End()
    {
        QueueFree();
    }
}
