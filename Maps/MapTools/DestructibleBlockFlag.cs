using Godot;
using System;
using System.Numerics;

public partial class DestructibleBlockFlag : Area2D
{
    [Export] Texture2D particleCutterTexture;
    public TileMapLayer tileMapLayer;
    public Vector2I tilePos;
    GpuParticles2D newParticlesNode;
    const int TILETEXTURESIZE = 20;

    public override void _Ready()
    {
        newParticlesNode = GetNode("GPUParticles2D").Duplicate() as GpuParticles2D;
        tilePos = new Vector2I((int) Position.X/16, (int) Position.Y/16);
    }

    public void Destroy()
    {
        if (IsQueuedForDeletion()) return;
        SpawnNewParticlesNode();

        tileMapLayer.SetCell(tilePos, -1); // deletes tile
        GD.Print("destroyed tile at position" + tilePos);
        QueueFree();
    }

    void SpawnNewParticlesNode()
    {
        newParticlesNode.OneShot = true;
        newParticlesNode.Finished += () => newParticlesNode.QueueFree();
        newParticlesNode.Texture = CreateParticlesTextureFromTilemapCoords(tilePos);
        newParticlesNode.Position = GlobalPosition;
        newParticlesNode.Emitting = true;

        Game.Instance.world.CallDeferred(MethodName.AddChild, newParticlesNode);
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
