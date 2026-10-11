using Godot;
using System;
using System.Xml.Schema;

public partial class MapEditor : Node2D
{
	TextureRect ghost = new TextureRect();
	Texture2D sheet = GD.Load<Texture2D>("uid://chyns5moafk7j");
	public override void _Ready()
	{
		ghost.Texture = new AtlasTexture()
		{
			Atlas = sheet,
			Region = new Rect2(Vector2.Zero, Vector2.One*20)	
		};
		
		AddChild(ghost);
	}

	public override void _Process(double delta)
	{
		Vector2 mousePos = GetGlobalMousePosition(); 
		if (mousePos.X < 0)
			mousePos = new Vector2( mousePos.X - 16, mousePos.Y); 
		if (mousePos.Y < 0)
			mousePos = new Vector2( mousePos.X, mousePos.Y - 16);
		ghost.Position = (new Vector2((int)(mousePos.X /16), (int)(mousePos.Y /16))*16) - Vector2.One * 2;
	}
}
