using Godot;
using System.Collections.Generic;
using System;
using System.Linq;

public partial class MagicMissile : HittableComponent
{
    [Export] AnimatedSprite2D sprite;

	public Player target;
    float acceleration = 1000f;

    float initialSpeed = 300f;
    float maxSpeed = 500f;
    Vector2 direction = Vector2.Left;
    Vector2 velocity;

    public override void _Ready()
    {
        base._Ready();

        if (target == null)
        {   // get random target
            target = GetRandomPlayer([]);
        }
        direction = (target.GlobalPosition - GlobalPosition).Normalized();
        velocity = direction * initialSpeed;
        Dictionary<int, Player> playerList = Game.Instance.playerNodesByInputIdx.ToDictionary();
        SetDeferred(PropertyName.target, playerList.ElementAt(GD.RandRange(0, playerList.Count() - 1)).Value);

        BodyEntered += onBodyDetected;
        GotHitvec += OnGotHit;
        target.died += OnTargetDead;
    }

    public override void _Process(double delta)
    {
        sprite.Rotation = velocity.Angle();
    }


    public override void _PhysicsProcess(double delta)
    {
        velocity -= velocity.Normalized() * acceleration/5 * (float)delta;
        velocity += (target.GlobalPosition - GlobalPosition).Normalized() * acceleration * (float)delta;
        
        if (velocity.LengthSquared() > maxSpeed * maxSpeed)
        {
            velocity = velocity.Normalized() * maxSpeed;
        }
        
        Position += velocity * (float)delta;
    }

    private void onBodyDetected(Node2D body)
    {
        if (body is not Player player) return;

        player.TakeDamage(target);
        QueueFree();
    }   

    void OnGotHit(Vector2 hitterPos)
    {
        velocity = (hitterPos - GlobalPosition).Normalized() * maxSpeed;

        target = GetRandomPlayer(target);
        
        maxSpeed *= 1.1f;
        acceleration *= 1.1f;

    }

    void OnTargetDead(Player player, Player killer)
    {
        target.died -= OnTargetDead;
        target = GetRandomPlayer(target);
        target.died += OnTargetDead;
    }

    Player GetRandomPlayer(Player exclude)
    {
        return GetRandomPlayer([exclude]);
    }
    Player GetRandomPlayer(Player[] excludeArray)
    {
        Dictionary<int, Player> playerList = Game.Instance.playerNodesByInputIdx.ToDictionary();
        
        foreach(Player player in Game.Instance.playerNodesByInputIdx.ToDictionary().Values)
        {
            if (player.IsDead) playerList.Remove(player.inputIdx);
        }

        foreach(Player player in excludeArray)
        {     
            playerList.Remove(player.inputIdx);
        }

        if (playerList.Count() <= 0)
        {
            GD.PrintErr("No other players found, returning current target.");
            return target;
        }

        return playerList.ElementAt(GD.RandRange(0, playerList.Count() - 1)).Value;
    }

}
