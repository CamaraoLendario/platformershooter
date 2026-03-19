using Godot;
using System;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

public partial class DestructibleBlockFlag : Area2D
{
    [Export] Texture2D particleCutterTexture;
    [Export] GpuParticles2D DestructionParticles;
    public TileMapLayer tileMapLayer;
    public Vector2I tilePos;
    const int TILETEXTURESIZE = 20;
    Node2D SpawnParentNode;

    public override void _Ready()
    {
        tilePos = new Vector2I((int) Position.X/16, (int) Position.Y/16);
        if (Game.Instance.world !=  null)
            SpawnParentNode = Game.Instance.world;
        else
        {
            SpawnParentNode = GetParent<Node2D>();
        }
        SetupDestructionParticles();
    }

    public bool Destroy()
    {
        if (IsQueuedForDeletion()) return false;
        DestructionParticles.Emitting = true;
        tileMapLayer.SetCell(tilePos, -1); // deletes tile
        GD.Print("destroyed tile at position" + tilePos);
        QueueFree();
        return true;
    }

    void SetupDestructionParticles()
    {
        DestructionParticles.Reparent(SpawnParentNode);
        DestructionParticles.OneShot = true;
        DestructionParticles.Texture = CreateParticlesTextureFromTilemapCoords(tilePos);
        DestructionParticles.Emitting = false;
        DestructionParticles.Finished += () => DestructionParticles.QueueFree();
    }

    ImageTexture CreateParticlesTextureFromTilemapCoords(Vector2I coords)
    {
        int cellSourceID = tileMapLayer.GetCellSourceId(coords);
        TileSetAtlasSource atlasSource = tileMapLayer.TileSet.GetSource(cellSourceID) as TileSetAtlasSource;
        Vector2I textureCoords = tileMapLayer.GetCellAtlasCoords(coords);
        Rect2I textureRegion = atlasSource.GetTileTextureRegion(textureCoords);
        Image imageTexture = atlasSource.Texture.GetImage();
        Image tileImageTexture = imageTexture.GetRegion(textureRegion);

        Image particleCutterImage = particleCutterTexture.GetImage();
        Image particlesTextureImage = Image.CreateEmpty(TILETEXTURESIZE, TILETEXTURESIZE, false, Image.Format.Rgba8);
        

        for(int Y = 0; Y < TILETEXTURESIZE; Y++)
        {
            for(int X = 0; X < TILETEXTURESIZE; X++)
            {
                if (particleCutterImage.GetPixel(X, Y).R == 0)
                {
                    Color tileTexturePixelColor = tileImageTexture.GetPixel(X, Y);
                    particlesTextureImage.SetPixel(X, Y, tileTexturePixelColor);    
                }
            }
        }
        
        return ImageTexture.CreateFromImage(particlesTextureImage);
    }
}
