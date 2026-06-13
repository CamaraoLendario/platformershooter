using Godot;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

public partial class MagicMissile : HittableComponent
{
    [Export] AnimatedSprite2D sprite;
	[Export] public Player target;

    Player ignoredTarget;
    Timer ignoredTargetGraceTimer = new()
    {
      OneShot = true,  
    };
    float gracePeriodTime = 0.5f;
    Vector2 targetPosition = Vector2.Zero;
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
        if (target != null)
        {
            targetPosition = target.GlobalPosition;        
            target.Died += OnTargetDead;
        }
        direction = (targetPosition - GlobalPosition).Normalized();
        velocity = direction * initialSpeed;
        
        SetDeferred(PropertyName.target, Game.Instance.players.ElementAt(GD.RandRange(0, Game.Instance.players.Count() - 1)));

        AddChild(ignoredTargetGraceTimer);
        ignoredTargetGraceTimer.Timeout += () =>
        {
            ignoredTarget = null;  
        };

        BodyEntered += OnBodyDetected;
        GotHit += OnGotHit;
        SignalBus.Instance.NewRoundStart += OnNewRoundStart;
    }

    public override void _Process(double delta)
    {
        sprite.Rotation = velocity.Angle();
    }

    public override void _PhysicsProcess(double delta)
    {
        velocity -= velocity.Normalized() * acceleration/5 * (float)delta;
        
        if (target != null && !target.isDead)
            targetPosition = target.Position;
        
        velocity += (targetPosition - GlobalPosition).Normalized() * acceleration * (float)delta;
        
        if (velocity.LengthSquared() > maxSpeed * maxSpeed)
        {
            velocity = velocity.Normalized() * maxSpeed;
        }
        
        Position += velocity * (float)delta;
    }

    private void OnBodyDetected(Node2D body)
    {
        if (body is not Player player) return;
        if (player == ignoredTarget) return;
        if (player.TakeDamage(target))
            End();
    }   

    void OnGotHit(Node2D hitter)
    {
        GD.Print("magicMissile got hit! hit by: ", hitter);
        ignoredTarget = target;
        ignoredTargetGraceTimer.Start(gracePeriodTime);

        if (hitter is LinearProjectile projectile)
        {
            velocity = projectile.Direction * maxSpeed;
        }
        else if (hitter is MeleeAttack melee)
        {
            velocity = -velocity.Normalized()  * maxSpeed;
        }
        else velocity = (hitter.GlobalPosition - GlobalPosition).Normalized() * maxSpeed;

        target.Died -= OnTargetDead;
        target = GetRandomPlayer(target);
        target.Died += OnTargetDead;

        maxSpeed *= 1.1f;
        acceleration *= 1.1f;
    }

    void OnTargetDead(Player player, Player killer)
    {
        target.Died -= OnTargetDead;
        target = GetRandomPlayer(target);
        target.Died += OnTargetDead;
    }

    Player GetRandomPlayer(Player exclude)
    {
        return GetRandomPlayer([exclude]);
    }
    Player GetRandomPlayer(Player[] excludeArray)
    {
        List<Player> playerList = Game.Instance.players.ToList();
        if(IsQueuedForDeletion()) return playerList[0];
        
        foreach(Player player in Game.Instance.players)
        {
            if (player.isDead) playerList.Remove(player);
        }

        foreach(Player player in excludeArray)
        {     
            playerList.Remove(player);
        }

        if (playerList.Count <= 0)
        {
            if (Game.Instance.GetAlivePlayerCount() > 0)
            {    
                GD.Print("No other players found, returning current target.");
            }
            else
            {
                GD.Print("All Players are Dead. running off stage..");
                //TODO this causes an error. trying to get a magic missile that doesn't exist? happens at the end of rounds
                targetPosition = /* Global */Position * 10000;
            }
            return target;
        }
        return playerList.ElementAt(GD.RandRange(0, playerList.Count() - 1));
    }

    void OnNewRoundStart()
    {
        End();
    }

    void End()
    {
        QueueFree();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DisconnectSignals();
    }

    void DisconnectSignals()
    {
        BodyEntered -= OnBodyDetected;
        GotHit -= OnGotHit;
        target.Died -= OnTargetDead;
        SignalBus.Instance.NewRoundStart -= OnNewRoundStart;
    }

}
