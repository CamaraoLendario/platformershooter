using Godot;
using GodotPlugins.Game;
using System;
using System.ComponentModel;

public partial class OverHeadDisplay : VBoxContainer
{
    //TODO: put code for power ups here
	[Export] HBoxContainer ammoHud;
	Player Main;

    public override void _Ready()
    {
		Main = GetParent<Player>();
		Main.ship.Shot += UpdateAmmoHUD;
		Main.ship.Reloaded += UpdateAmmoHUD;
		Main.ship.Started += Show;
		Main.ship.Ended += Hide;
    }

	void UpdateAmmoHUD()
	{
		GD.Print("updating ammo");
		for(int i = 0; i < ShipAttack.MAXAMMO; i++)
		{
			AtlasTexture textureRectAtlas = ammoHud.GetChild<TextureRect>(i).Texture as AtlasTexture;

			if (i < Main.ship.currentAmmo)
			{
				textureRectAtlas.Region = new Rect2(Vector2.Zero, new Vector2(4, 4)); 
			}
			else
			{
				textureRectAtlas.Region = new Rect2(new Vector2(4, 0), new Vector2(4, 4)); 
			}
		}
	}
}
