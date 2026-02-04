using Godot;
using Godot.Collections;
using System;
using System.Collections;

[Tool]
public partial class WeaponPickup : Area2D
{
	[Export] Array<PackedScene> weapons = [
		GD.Load<PackedScene>("uid://bvv0r0rral008"),	// Ice
		GD.Load<PackedScene>("uid://bklg56414tpal"),	// Fire
		GD.Load<PackedScene>("uid://duuadcioquoip"),	// Poison
        GD.Load<PackedScene>("uid://bvm8sifpder5k"),	// Wind
		GD.Load<PackedScene>("uid://d0lhsiquci2f5"),	// Rock
    ];
	[Export(PropertyHint.Enum, "Ice:0, Fire:1, Poison:2, Wind:3, Rock:4")]
	public int chosenWeapon
    {
		get
		{
			return ChosenWeapon;
		}
        set
        {
			ChosenWeapon = value;
			GetNode<AnimatedSprite2D>("ElementSprite").Animation = weaponNamesArray[value];
        }
    }
	AnimatedSprite2D weaponSprite;
	CollisionShape2D collision;
	private int ChosenWeapon;
	private string[] weaponNamesArray = ["Ice", "Fire", "Poison", "Wind", "Rock"];
	Timer cooldownTimer = new Timer();
	float cooldownTime = 10.0f;

	public override void _Ready()
	{
		weaponSprite = GetNode<AnimatedSprite2D>("ElementSprite");
		collision = GetNode<CollisionShape2D>("CollisionShape");
		weaponSprite.Play();
		if (GetParent<Node>().Name != "WeaponPickups" ) GD.PrintErr("WEAPON PICKUPS NODE NOT DETECTED: weapon pickups need to be placed under the weapon pickups Node");
		if (Engine.IsEditorHint()) return;
		cooldownTimer.OneShot = true;
		AddChild(cooldownTimer);
		cooldownTimer.Timeout += endCooldown;
		BodyEntered += OnBodyDetected;
		weaponSprite.Animation = weaponNamesArray[chosenWeapon];

	}

	void OnBodyDetected(Node2D body)
	{
		if (Engine.IsEditorHint()) return;
		if (!(body is Player) || (body is Player player) && player.pilot.weaponHolder.CurrentWeapon != null) return;

		PilotAttack playerPilot = (body as Player).pilot;

		playerPilot.SetWeapon(weapons[chosenWeapon].Instantiate<Weapon>());
		EnterCooldown();
	}

	void EnterCooldown()
	{
		cooldownTimer.Start(cooldownTime);
		weaponSprite.Hide();
		collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	void endCooldown()
	{
		weaponSprite.Show();
		collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
	}

	public void CheckForPlayers()
	{
		if (Engine.IsEditorHint()) return;
		if (!cooldownTimer.IsStopped()) return;

		foreach(Node2D body in GetOverlappingBodies())
        {
            if (body is Player)
            {
				OnBodyDetected(body);
				return;
            }
        }
    }
}
