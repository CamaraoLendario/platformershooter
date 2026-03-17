using Godot;
using System;
using System.Collections.Generic;

public partial class Blackhole : Node2D
{
	[Export] float baseGravityForce = 100f;
	[Export] Area2D pullArea;
	float pullAreaRadius = 0f;
	List<Node2D> objectsWithinRange = [];

	void OnAreaEntered(Node2D obj){objectsWithinRange.Add(obj);}
	void OnAreaExited(Node2D obj){objectsWithinRange.Remove(obj);}
	void OnBodyEntered(Node2D obj){
		objectsWithinRange.Add(obj);
	}
	void OnBodyExited(Node2D obj){
		objectsWithinRange.Remove(obj);
		if (obj is Player player){
			player.pilot.gravityForce = PilotController.BASEGRAVITYFORCE;
		}
	}

    public override void _PhysicsProcess(double delta)
    {
		foreach(Node2D node in objectsWithinRange)
		{
			float gravityMultiplier = (GlobalPosition - node.GlobalPosition).Length() / pullAreaRadius;
			float gravityForce = baseGravityForce * gravityMultiplier * (float)delta;
			Vector2 pullForce = (GlobalPosition - node.GlobalPosition).Normalized() * gravityForce;
			if (node is Player player)
			{
				player.pilot.gravityForce *= 0;
				player.Velocity += pullForce;		
			}
			else if (node is LinearProjectile projectile)
			{
				projectile.direction = ((projectile.direction * projectile.speed) + pullForce).Normalized();
			}
		}
    }
    public override void _Ready()
    {
        ConnectSignals();
		foreach(Node child in pullArea.GetChildren())
		{
			if (child is CollisionShape2D shape && shape.Shape is CircleShape2D circle)
			{
				pullAreaRadius = circle.Radius;
			}
		}
    }
	void ConnectSignals()
	{
		pullArea.AreaEntered += OnAreaEntered;
		pullArea.AreaExited += OnAreaExited;
		pullArea.BodyEntered += OnBodyEntered;
		pullArea.BodyExited += OnBodyExited;
	}
}
