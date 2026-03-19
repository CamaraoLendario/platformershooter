using Godot;
using System;
using System.Collections.Generic;

public partial class ShipPowerUpChest : Node2D
{
	[Signal] public delegate void OpenedEventHandler();
	private bool exists = false;
	[Export] float respawnTime = 20f;
	[ExportGroup("Nodes")]
	[Export] HittableComponent hittableComponent;
	[Export] AnimationPlayer animationPlayer;
	bool isOrphan = false;
	Timer spawnCooldownTimer = new();
	Node2D world;
	Map map;
	PackedScene shipPowerUpPickup = GD.Load<PackedScene>("uid://bgvvii5wegfxh");

    public override void _Ready()
    {
		map = Game.Instance.selectedMap;
		world = Game.Instance.world;
		if (world == null)
		{
			world = map;
		}
	    hittableComponent.GotHit += OnHit;
		Despawn();
		
		if (GetParent() is not ShipPowerUpChestSpawner)
		{
			Spawn();
			isOrphan = true;
			spawnCooldownTimer.OneShot = true;
			AddChild(spawnCooldownTimer);
		}
    }

	public void Spawn()
	{
		Visible = true;
		hittableComponent.Enabled = true;
		if (map.IsPositionInPilotArea(this.GlobalPosition))
			animationPlayer.Play("Spawn");
		else
			animationPlayer.Play("SpawnSpace");
	}
	public void Despawn()
	{
		Visible = false;
		hittableComponent.Enabled = false;
	}

	void OnHit(Node2D hitter)
	{
		OpenChest();
	}

	void OpenChest()
	{
		ShipPowerUpPickup newPowerUpPickup = shipPowerUpPickup.Instantiate<ShipPowerUpPickup>();
		newPowerUpPickup.Position = Position + (Vector2.Up * 16);
		newPowerUpPickup.HeldPowerup = GD.RandRange(0, newPowerUpPickup.powerUps.Count - 1);

		world.CallDeferred(MethodName.AddChild, newPowerUpPickup);
		EmitSignal(SignalName.Opened);
		Despawn();

		if (isOrphan)
		{
			spawnCooldownTimer.Start(respawnTime);
		}
	}
}
