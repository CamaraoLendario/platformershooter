using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Map : Node2D
{
    [Export] public float TimeToSuddenDeath = 30f;
    [ExportGroup("Nodes")]
    [Export] public Node Pickups;
    [Export] public MapBorders MapBordersNode;
    [Export] public PilotArea pilotArea;
    [Export] public MapCamera camera;
    InteractableTiles currentTileMapLayerNode;
    public Vector2 PixelsMapSize
    {
        get
        {
            return MapBordersNode.pixelMapSize;
        }
        private set
        {
            return;
        }
    }

    public override void _Ready() 
    {
        Game.Instance.NewRoundStarted += OnNewRoundStarted;
        currentTileMapLayerNode = GetNode<InteractableTiles>("InteractableTiles");
        //ReplicateCollisions();
    }
    
    void OnNewRoundStarted()
    {
        currentTileMapLayerNode.Reset();
        ForcePickupsReset();
    }

    public bool IsPositionInPilotArea(Vector2 Position)
    {
        return pilotArea.IsInPilotArea(Position);
    }

    void ForcePickupsReset()
    {
        foreach(Node Node in Pickups.GetChildren())
        {
            if (Node is WeaponPickup weaponPickup)
            {
                weaponPickup.EndCooldown();
            }
            else if (Node is ShipPowerUpChestSpawner shipPowerUpChest)
            {
                foreach(Node child in shipPowerUpChest.GetChildren())
                {
                    if (child is not ShipPowerUpChest shipPowerupChest) continue;
                    shipPowerupChest.Despawn();
                }
            }

        }
    }

    public override void _ExitTree()
    {
        DisconnectSignals();
        base._ExitTree();
    }

    void DisconnectSignals()
    {
        Game.Instance.NewRoundStarted -= OnNewRoundStarted;
    }

    void ReplicateCollisions()
    {
        foreach(Player player in Game.Instance.playerNodesByColor.Values)
        {
            foreach (CollisionShape2D col in player.collisionShapes)
            {
                for(int x = -1; x < 2; x++)
                {
                    for(int y = -1; y < 2; y++)
                    {
                        if (x == 0 && y == 0) continue;
                        CollisionShape2D newCol = col.Duplicate((int) DuplicateFlags.Groups) as CollisionShape2D;
                        newCol.Position += new Vector2(x, y) * 10;
                        player.AddChild(newCol);
                    }
                }
            }
        }
    }
}