using Godot;
using GodotPlugins.Game;
using System;
using System.ComponentModel;

public partial class ShipPowerUpsHolder : Node2D
{
	[ExportGroup("Nodes")]
	[Export] TextureRect drillIcon;
	[Export] TextureRect speedIcon;
	[Export] TextureRect dualGunsIcon;
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

	public bool AddPowerUp(ShipPowerUp powerUp, int powerUpID)
	{
		foreach (ShipPowerUp ownedPowerUp in GetChildren())
		{
			if (ownedPowerUp.powerUpID == powerUpID)
			{
				return false;
			}
		}
		
		ShowIcon(powerUp);
		powerUp.powerUpID = powerUpID;
		AddChild(powerUp);
		return true;
	}
	
	void ShowIcon(ShipPowerUp powerUp)
	{
		if (powerUp is ShipPowerUpDrillShot)
		{
			drillIcon.Show();
		}
		else if (powerUp is ShipPowerUpSpeedUp)
		{
			speedIcon.Show();
		}
		else if (powerUp is ShipPowerUpDualGuns)
		{
			dualGunsIcon.Show();
		}
	}

	void HideIcon(ShipPowerUp powerUp)
	{
		if (powerUp is ShipPowerUpDrillShot)
		{
			drillIcon.Hide();
		}
		else if (powerUp is ShipPowerUpSpeedUp)
		{
			speedIcon.Hide();
		}
		else if (powerUp is ShipPowerUpDualGuns)
		{
			dualGunsIcon.Hide();
		}
	}

    private void OnNewPowerUp(Node node)
	{
		if (node is not ShipPowerUp powerUp) return;
	}

    private void OnPowerUpLost(Node node)
	{
		if (node is not ShipPowerUp powerUp) return;
		HideIcon(powerUp);
	}

	void Clear()
	{
		foreach(Node node in GetChildren())
		{
			if (node is not ShipPowerUp shipPowerUp) return;

			shipPowerUp.End();
		}
	}
}
