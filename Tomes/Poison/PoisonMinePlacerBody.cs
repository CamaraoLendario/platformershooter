using Godot;
using System;

public partial class PoisonMinePlacerBody : CharacterBody2D
{
	[Export] Sprite2D sprite;
	[Export] PackedScene poisonMineScene;
	public float speed;
	public Vector2 direction;
	public Player owner;
	const float GRAVITYFORCE = 1250;
	Vector2 velocity;
	Map currentMap;
	bool isInPilotArea = true;
	World world;

    public override void _Ready()
    {
        velocity = direction * speed;
		sprite.Rotation = direction.Angle();
		currentMap = Game.Instance.currentMap;
		SignalBus.Instance.NewRoundStarted += OnNewRoundStarted;
		world = GetTree().GetFirstNodeInGroup("World") as World;
	}

	void OnNewRoundStarted()
	{
		QueueFree();
	}

    public override void _PhysicsProcess(double delta)
    {
		isInPilotArea = currentMap.IsPositionInPilotArea(Position);
		if (isInPilotArea) processGravity((float)delta);
		velocity -= velocity.Normalized() * velocity.Length() * 0.75f * (float)delta;

		sprite.Rotation = velocity.Angle();

        KinematicCollision2D collisionInfo = MoveAndCollide(velocity * (float)delta);
		CheckCollision(collisionInfo);
    }
	
	void processGravity(float delta)
    {
        velocity += new Vector2(0, 1) * GRAVITYFORCE * delta;
    }

	void CheckCollision(KinematicCollision2D collisionInfo)
    {
        if (collisionInfo == null || collisionInfo.GetCollider() is not TileMapLayer) return;

		SummonMine(collisionInfo);
		QueueFree();
    }

	void SummonMine(KinematicCollision2D collisionInfo)
	{
		PoisonMine newPoisonMine = poisonMineScene.Instantiate<PoisonMine>();
		newPoisonMine.wallSide = collisionInfo.GetNormal();
		newPoisonMine.Position = collisionInfo.GetPosition();
		newPoisonMine.owner = owner;

		world.AddChild(newPoisonMine);
	}

    public override void _ExitTree()
    {
        base._ExitTree();
		SignalBus.Instance.NewRoundStarted -= OnNewRoundStarted;
    }

}
