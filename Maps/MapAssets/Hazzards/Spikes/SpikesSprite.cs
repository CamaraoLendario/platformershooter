using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class SpikesSprite : Node2D
{
	[Signal] public delegate void sizeUpdatedEventHandler(int Length);
	[ExportGroup("spriteSettings")]
	[Export] Texture2D spikesSpritesheet;
	[ExportGroup("References")]
	[Export] Marker2D leftMarker = null; 
	[Export] Marker2D rightMarker = null;
	List<Marker2D> markers = new();
	public List<AtlasTexture> atlasTextures = new();
	Node2D spritesNode;
	List<Sprite2D> sprites = new();

    public override void _Ready()
    {
		spritesNode = new()
		{
			Name = "Sprites",
		};
		AddChild(spritesNode);

		SetupAtlasTextures();

		markers.Add(leftMarker);
		markers.Add(rightMarker);
    }

	void SetupAtlasTextures()
	{
		for(int i = -1; i <= 1; i++)
		{
			AtlasTexture newAtlasTexture = new AtlasTexture()
				{
					Atlas = spikesSpritesheet,
					Region = new(new Vector2(16*(i+1), 0), new Vector2(16, 16)),
				};
			atlasTextures.Add(newAtlasTexture);
		}
	}

	public void ResetSize()
	{
		leftMarker.Position = Vector2.Zero;
		rightMarker.Position = Vector2.Right * 16;

		syncSprites();
	}

    public override void _Process(double delta)
    {
        foreach(Marker2D marker in markers)
		{
			marker.Position *= Vector2.Right;
		}

		syncSprites();
    }


	void syncSprites()
	{
		float Length = -leftMarker.Position.X + rightMarker.Position.X;
		int neededSpritesCount = (int)(Length/8);
		int spriteCountOffset = neededSpritesCount - sprites.Count;
		if(spriteCountOffset == 0) return;
		
		if (spriteCountOffset > 0)
		{
			for (int i = 0; i < spriteCountOffset; i++)
			{
				Sprite2D newSprite = GetNewSprite();
				spritesNode.AddChild(newSprite);
				sprites.Add(newSprite);
			}
		}
		else
		{	
			for (int i = 0; i < -spriteCountOffset; i++)
			{
				Sprite2D sprite = sprites.Last();
				sprites.Remove(sprite);
				sprite.QueueFree();
			}
		}
		for (int i = 2; i < sprites.Count; i++)
		{
			Sprite2D Sprite = sprites[i];
			Sprite.Position = Vector2.Right * (((i-1) * 8)+4);
			Sprite.Texture = atlasTextures[1];
		}

		for (int i = 0; i <= 1; i++)
		{
			Sprite2D Sprite = sprites[i];
			Sprite.Position = Vector2.Right * ((i * 8 * (neededSpritesCount-1))+4);
			Sprite.Texture = atlasTextures[i*2];
		}
		
		EmitSignal(SignalName.sizeUpdated, neededSpritesCount);
	}

	Sprite2D GetNewSprite()
	{
		Sprite2D newSprite = new()
		{
			Offset = Vector2.Up * 8,
		};

		return newSprite;
	}
}