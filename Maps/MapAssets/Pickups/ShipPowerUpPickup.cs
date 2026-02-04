using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class ShipPowerUpPickup : Area2D
{
	[Export] Array<PackedScene> powerUps = [
		GD.Load<PackedScene>("uid://b7rfhvwq15fsc"),	// DualGuns
		GD.Load<PackedScene>("uid://5c4yyxfl8m1b"),		// SpeedUp
		GD.Load<PackedScene>("uid://b4uo2lmmg5ov1"),	// Laser
		GD.Load<PackedScene>("uid://c3shcbl7mtyrs"),	// DrillShot
    ];
	[Export(PropertyHint.Enum, "DualGuns:0, SpeedUp:1, Laser:2, DrillShot:3, ")]
	public int HeldPowerup
    {
		get
		{
			return heldPowerup;
		}
        set
        {
			heldPowerup = value;
			GetNode<AnimatedSprite2D>("PowerUpSprite").Animation = weaponNamesArray[value];
        }
    }
	private int heldPowerup;
	private string[] weaponNamesArray = ["DualGuns", "SpeedUp", "Laser", "DrillShot", ];
	Timer CooldownTimer;
	const float COOLDOWNTIME = 15;
    public override void _Ready()
    {
		BodyEntered += OnBodyEntered;
	}

	void OnBodyEntered(Node2D body)
	{
		if (body is not Player player) return;

		if (player.ship.AddPowerUp(powerUps[HeldPowerup].Instantiate<ShipPowerUp>()))
		{
			QueueFree();
		}
	}
}
