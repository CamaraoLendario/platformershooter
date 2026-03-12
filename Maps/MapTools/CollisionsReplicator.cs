using Godot;
using System.Collections.Generic;
using System;

[Tool]
public partial class CollisionsReplicator : Area2D
{
	public Map main;
	public MapBorders parent;
	public Vector2 side;
	List<Node2D> currentBodies = new();
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
		parent = GetParent<MapBorders>();
	}

	void OnBodyEntered(Node2D body)
	{
		if (currentBodies.Contains(body)) return;
		foreach(CollisionsReplicator replicator in parent.GetChildren())
		{
			replicator.currentBodies.Add(body);
		}
		
	}

	void OnBodyExited(Node2D body)
	{
		if (!currentBodies.Contains(body)) return;
		foreach(CollisionsReplicator replicator in parent.GetChildren())
		{
			replicator.currentBodies.Remove(body);
		}
	}

	void ReplicateCollision()
	{
		
	}
}
