using Godot;
using System;
using System.Collections.Generic;

public partial class CollisionReplicator : Area2D
{
	List<(CharacterBody2D replicatedBody, Player player, Vector2 dir)> replicatedBodies = [];
	public Vector2 side;
	MapBorders boundsHandler;
    public override void _Ready()
	{
		boundsHandler = GetParent() as MapBorders;
		BodyEntered += OnBodyOutOfBounds;
		BodyExited += OnBodyInBounds;
    }

	void OnBodyOutOfBounds(Node2D body)
	{
		if (!(body is Player player)) return;
		GD.Print("body in");

		CharacterBody2D replicatedBody = new CharacterBody2D();
		Game.Instance.world.CallDeferred(MethodName.AddChild, replicatedBody);
		foreach (CollisionShape2D shape in player.collisionShapes)
		{
			CollisionShape2D replicatedShape = shape.Duplicate() as CollisionShape2D;
			replicatedShape.Position = shape.Position + boundsHandler.pixelMapSize * -side;
			replicatedBody.CallDeferred(MethodName.AddChild, replicatedShape);
			replicatedBodies.Add((replicatedBody, player, side));
		}
	}

	void OnBodyInBounds(Node2D body)
    {
		if (!(body is Player player)) return;
		GD.Print("body out");
		
		List<(CharacterBody2D replicatedBody, Player player, Vector2 dir)> copyOfReplicatedBodies = replicatedBodies;
		for(int i = 0; i <= copyOfReplicatedBodies.Count -1; i++)

			if (player == copyOfReplicatedBodies[i].player)
			{
				replicatedBodies[i].replicatedBody.QueueFree();
				replicatedBodies.RemoveAt(i);	
			}
    }

	public override void _PhysicsProcess(double delta)
	{
		List<(CharacterBody2D replicatedBody, Player player, Vector2 dir)> copyOfReplicatedBodies = replicatedBodies;
		for(int i = 0; i <= copyOfReplicatedBodies.Count -1; i++)
		{
			replicatedBodies[i].replicatedBody.Position = replicatedBodies[i].player.Position * boundsHandler.pixelMapSize * -replicatedBodies[i].dir;
		}
	}
}
