using Godot;
using System.Collections.Generic;
using System;
using System.Linq;

public partial class MagicMissile : HittableComponent
{
    [Export] AnimatedSprite2D sprite;

	Player target;
    float acceleration = 1000f;

    float initialSpeed = 300f;
    float maxSpeed = 500f;
    Vector2 direction = Vector2.Left;
    Vector2 velocity;

    public override void _Ready()
    {
        base._Ready();
        velocity = direction * initialSpeed;
        Dictionary<int, Player> playerList = Game.Instance.playerNodesByInputIdx.ToDictionary();
        SetDeferred(PropertyName.target, playerList.ElementAt(GD.RandRange(0, playerList.Count() - 1)).Value);
        GD.Print(target);

        BodyEntered += onBodyDetected;
        GotHitvec += OnGotHit;
    }

    public override void _Process(double delta)
    {
        sprite.Rotation = velocity.Angle();
    }


    public override void _PhysicsProcess(double delta)
    {
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
        GD.Print("GOT HIT!!!");
        velocity = (hitterPos - GlobalPosition).Normalized() * maxSpeed;
        Dictionary<int, Player> playerList = Game.Instance.playerNodesByInputIdx.ToDictionary();
        playerList.Remove(target.inputIdx);

        if (playerList.Count() <= 0)
        {
            GD.PrintErr("No other players found, maintaining current target.");
            return;
        }

        target = playerList.ElementAt(GD.RandRange(0, playerList.Count() - 1)).Value;
        maxSpeed += 50f;
    }
}
