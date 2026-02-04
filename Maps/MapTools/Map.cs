using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Map : Node2D
{
    [Export] public float TimeToSuddenDeath = 120f;
    [ExportGroup("Nodes")]
    [Export] public Node weaponPickups;
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
    }
    
    void OnNewRoundStarted()
    {
        currentTileMapLayerNode.Reset();
    }

    public bool IsPositionInPilotArea(Vector2 Position)
    {
        return pilotArea.IsInPilotArea(Position);
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

}