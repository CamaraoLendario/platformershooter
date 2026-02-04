using Godot;
using System;
using System.ComponentModel;

public partial class ShipPowerUpsHolder : Node2D
{
	[ExportGroup("Nodes")]
	[Export] public ShipAttack controller;

    public override void _Ready()
    {
        base._Ready();
		ChildEnteredTree += OnNewPowerUp;
		ChildExitingTree += OnPowerUpLost;
    }

	public bool HasDifferentShootingMechanics()
	{
		foreach(ShipPowerUp powerUp in GetChildren())
		{
			if (powerUp is ShipGunPowerUp)
			{
				return true;
			}
		}
		return false;
	}

	public bool AddPowerUp(ShipPowerUp powerUp)
	{
		foreach (ShipPowerUp ownedPowerUp in GetChildren())
		{
			if (powerUp.GetClass() == ownedPowerUp.GetClass())
			{
				return false;
			}
		}
		AddChild(powerUp);
		return true;
	}
	
    private void OnNewPowerUp(Node node)
	{
	}

    private void OnPowerUpLost(Node node)
	{
	}
}
