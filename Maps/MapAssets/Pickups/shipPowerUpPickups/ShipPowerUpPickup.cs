using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class ShipPowerUpPickup : Pickup
{
	[Export] public Array<PackedScene> powerUps = [
		GD.Load<PackedScene>("uid://b7rfhvwq15fsc"),	// DualGuns
		GD.Load<PackedScene>("uid://5c4yyxfl8m1b"),		// SpeedUp
		GD.Load<PackedScene>("uid://c3shcbl7mtyrs"),	// DrillShot
		//GD.Load<PackedScene>("uid://b4uo2lmmg5ov1"),	// Laser
    ];
	[Export(PropertyHint.Enum, "DualGuns:0, SpeedUp:1, DrillShot:2"/* , Laser:3, " */)]
	public int HeldPowerup
    {
		get
		{
			return heldPowerup;
		}
        set
        {
			heldPowerup = value;
			itemName = weaponNamesArray[value];
			if (Engine.IsEditorHint())
				GetNode<AnimatedSprite2D>("PowerUpSprite").Animation = weaponNamesArray[value];
        }
    }
	private int heldPowerup;
	private string[] weaponNamesArray = ["DualGuns", "SpeedUp", "DrillShot",/*  "Laser",  */];
	Timer CooldownTimer;
	const float COOLDOWNTIME = 15;
    public override void _Ready()
    {
		
		itemName = weaponNamesArray[HeldPowerup];
		BodyEntered += OnBodyEntered;
		Game.Instance.NewRoundStarted += QueueFree;
		GetNode<AnimatedSprite2D>("PowerUpSprite").Animation = weaponNamesArray[HeldPowerup];
	}

	void OnBodyEntered(Node2D body)
	{
		if (body is not Player player) return;

		bool wasPickSuccessful = player.ship.AddPowerUp(powerUps[HeldPowerup].Instantiate<ShipPowerUp>(), HeldPowerup);

		if (wasPickSuccessful)
		{
			SummonNotification(player);
			QueueFree();
		}
	}

    public override void _ExitTree()
    {
		BodyEntered -= OnBodyEntered;
		Game.Instance.NewRoundStarted -= QueueFree;
        base._ExitTree();
    }

}
