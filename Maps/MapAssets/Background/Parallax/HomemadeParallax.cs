using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
[Tool]
public partial class HomemadeParallax : Node2D
{
    [Export] float Size
    {
        get
        {
            return size;
        }
        set
        {
            size = value;
            SetSideLength();
        }
    }
    float size = 1;
    [Export] Vector2 ScrollDir
    {
        get
        {
            return scrollDir;
        }
        set
        {
            scrollDir = value;
            SetScrollDir();
        }
    }
    Vector2 scrollDir = new(1, 0f);
    [Export] float ScrollSpeed
    {
        get
        {
            return scrollSpeed;
        }
        set
        {
            scrollSpeed = value;
            SetScrollSpeed();
        }
    }

    float scrollSpeed = 0f;
    [Export] float ParallaxMultiplier
    {
        get
        {
            return parallaxMultiplier;
        }
        set
        {
            parallaxMultiplier = value;
            SetParallaxMultiplier();
        }
    }
    float parallaxMultiplier = 1f;

    [Export] bool Repeats
    {
        get
        {
            return repeats;
        }
        set
        {

            repeats = value;
            SetRepeats();
        }
    }
    bool repeats = true;
    [ExportGroup("Assets")]
    [Export] ShaderMaterial currentShaderMaterial;
    public Camera2D currentCamera;

    public override void _Ready()
    {
        currentCamera = GetViewport().GetCamera2D();
        foreach(Node child in GetChildren()){
            if (child is not Sprite2D sprite) continue;
            Sprite2D backgroundSprite = sprite;
            AtlasTexture backgroundSpriteTexture = backgroundSprite.Texture as AtlasTexture;
            Rect2 textureRect2 = backgroundSpriteTexture.Region;
            Vector4 textureRegion = new(
                textureRect2.Position.X,
                textureRect2.Position.Y,
                textureRect2.Size.X,
                textureRect2.Size.Y
            );

            ShaderMaterial newShaderMaterial = currentShaderMaterial.Duplicate() as ShaderMaterial;

            newShaderMaterial.SetShaderParameter("textureRegion", textureRegion);
            newShaderMaterial.SetShaderParameter("texSize", backgroundSpriteTexture.Atlas.GetSize());
            sprite.Material = newShaderMaterial; 
            sprite.UseParentMaterial = false;
        }
        SetupShaders();
    }
    public override void _Process(double delta)
    {
        if (!repeats && !Engine.IsEditorHint())
        {
            Vector2 cameraDistFromOrigin = currentCamera.GlobalPosition;
            //Position = -cameraDistFromOrigin;
            Position = cameraDistFromOrigin * parallaxMultiplier;
        }
    }

    void SetSideLength()
    {
        foreach(Node child in GetChildren()){
                if (child is not Sprite2D sprite) continue;
                (sprite.Material as ShaderMaterial).SetShaderParameter("sideLength", Size);
            }
        Scale = Vector2.One * Size;
    }
    void SetScrollDir()
    {
        foreach(Node child in GetChildren()){
                if (child is not Sprite2D sprite) continue;
                (sprite.Material as ShaderMaterial).SetShaderParameter("scrollDir", ScrollDir);
            }
    }
    void SetScrollSpeed()
    {
        foreach(Node child in GetChildren()){
                if (child is not Sprite2D sprite) continue;
                (sprite.Material as ShaderMaterial).SetShaderParameter("scrollSpeed", ScrollSpeed);
            }
    }
    void SetParallaxMultiplier()
    {
        foreach(Node child in GetChildren()){
                if (child is not Sprite2D sprite) continue;
                (sprite.Material as ShaderMaterial).SetShaderParameter("parallaxMultiplier", ParallaxMultiplier);
            }
    }
    void SetRepeats()
    {
        foreach(Node child in GetChildren()){
                if (child is not Sprite2D sprite) continue;
                (sprite.Material as ShaderMaterial).SetShaderParameter("repeats", Repeats);
            }
    }
    void SetupShaders()
    {
        SetSideLength();
        SetScrollDir();
        SetScrollSpeed();
        SetParallaxMultiplier();
        SetRepeats();
    }
}
