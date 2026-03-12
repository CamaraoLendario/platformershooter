using Godot;
using System;
using System.ComponentModel;

[Tool]
public partial class Spikes : Area2D
{
    [ExportToolButton("Reset")]
    public Callable ResetButton => Callable.From(Reset);
    [Export] SpikesSprite spriteHandler;
    public CollisionShape2D collisionShape;
    Timer spikesTimer = new();
    public override void _Ready()
    {
        spikesTimer.OneShot = true;
        spikesTimer.Timeout += Spike;
        AddChild(spikesTimer);
        PrepareCollision();
        spriteHandler.sizeUpdated += UpdateCollisionSize;
        BodyEntered += OnBodyEntered;
    }

    void Reset()
    {
        spriteHandler.ResetSize();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not Player player) return;

        if (GetFrame() != 2)
        {
            SetFrame(1);
            spikesTimer.Start(0.5);
        }
        else
        {
            DealDamage(player);
        }
    }

    void Spike()
    {
        if (GetFrame() != 2)
        {
            SetFrame(2);
            foreach(Node2D body in GetOverlappingBodies())
            {
                if (body is not Player player) continue;
                DealDamage(player);
            }
            spikesTimer.Start(0.5);
        }
        else
        {
            SetFrame(0);
        }
        
    }
    void DealDamage(Player player)
    {
        player.TakeDamage();
        player.Velocity += Vector2.Up.Rotated(Rotation) * 300;
    }
    void PrepareCollision()
    {
        CollisionShape2D newCollision = new()
        {
            Shape = new RectangleShape2D()
            {
                Size = new Vector2(
                    0, 
                    4
                )
            },
            Position = Vector2.Up * 2
        };
        collisionShape = newCollision;
        AddChild(newCollision);
    }

    void UpdateCollisionSize(int size)
    {
        Vector2 newSize = new Vector2(size*8, 4);

        collisionShape.Shape =  new RectangleShape2D()
        {
            Size = newSize,
        };

        collisionShape.Position = (newSize / 2) * new Vector2(1, -1);
    }

    int GetFrame()
    {
        return (int)spriteHandler.atlasTextures[0].Region.Position.Y / 16;
    }
    
    void SetFrame(int frame)
    {
        foreach (AtlasTexture atlasTexture in spriteHandler.atlasTextures)
        {
            atlasTexture.Region = new Rect2(new Vector2(atlasTexture.Region.Position.X, frame * 16), atlasTexture.Region.Size);
        }
    }
}