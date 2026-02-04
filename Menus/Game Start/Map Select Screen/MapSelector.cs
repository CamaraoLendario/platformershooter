using Godot;
using SpaceMages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

public partial class MapSelector : Control
{
	[Export] MapPreviewsContainer mapPreviewsContainer;
	[Export] Texture2D pilotTexture; // TODO find a better way to store the pilot sprites, there are gonna be multiple different pilots
	Shader pixelOutlineShader = GD.Load("uid://cywx8daesh6uu") as Shader;
	Shader mapSelectedShader = GD.Load("uid://dha1qnh4th4vi") as Shader;
	public bool isCommenced = false;
	[Export] PackedScene[] mapscenes;
	public List<Map> maps = [];
	Color Black = new Color(0, 0, 0);
	Color White = new Color(1, 1, 1);
	int maxMapWidth = 52;
	int maxMapHeight = 30;
	public const string SELECTINGNAME = "playersSelectingContainter";
	public const string SELECTEDNAME = "playersSelectedContainter";
	List<Dictionary<string, int>> playerInfoList; 
	Dictionary<string, Color> tilesMapColor = new()
	{
		["Grass"] = new Color(128f/255f, 80f/255f, 38f/255f),
		["Factory"] = new Color(65f/255f, 62f/255f, 68f/255f),
		["Amethyst"] = new Color(144f/255f, 68f/255f, 150f/255f),
		["Rock"] = new Color(74f/255f, 83f/255f, 89f/255f),
		["Planks"] = new Color(117f/255f, 87f/255f, 56f/255f),
		["Glass"] = new Color(203f/255f, 219f/255f, 252f/255f)
	};

	public override void _Ready()
    {
		maxMapWidth += 4;
		maxMapHeight += 4;
		mapPreviewsContainer.Main = this;
		GenerateMapPreviews();
		Position = new Vector2(0, -(float)ProjectSettings.GetSetting("display/window/size/viewport_height"));
	}
	
	public void Commence(List<Dictionary<string, int>> playerInfoList)
	{
		if (isCommenced) return;
		isCommenced = true;

		this.playerInfoList = playerInfoList;

		foreach (Dictionary<string, int> player in playerInfoList)
        {
			Control newSelectorPointer = new Control();
			Sprite2D newSelectorSprite = new Sprite2D();
			newSelectorSprite.ZIndex = 1;
			newSelectorPointer.AddChild(newSelectorSprite);
			newSelectorPointer.CustomMinimumSize = pilotTexture.GetSize() * 4;
			newSelectorSprite.Scale = Vector2.One * 4;
			newSelectorSprite.Offset = new Vector2(8, 8.5f);
			newSelectorSprite.Texture = pilotTexture;
			ShaderMaterial material = new ShaderMaterial();
			material.Shader = pixelOutlineShader.Duplicate() as Shader;
			material.SetShaderParameter("Color", SpaceMagesVars.teamColors[player["colorIdx"]]);
			newSelectorSprite.Material = material;

            mapPreviewsContainer.AddMapSelector(newSelectorPointer, player["inputIdx"]);
        }
		OpenMapSelector();
    }

	void GenerateMapPreviews()
    {
        foreach(PackedScene mapscene in mapscenes)
        {
            maps.Add(mapscene.Instantiate<Map>());
        }

		for (int i = 0; i < maps.Count; i++)
        {
			TextureRect mapPreview = new TextureRect();
            mapPreview.Texture = GetMapTexturePreview(maps[i]);
			mapPreview.CustomMinimumSize = new Vector2(maxMapWidth * 8, maxMapHeight * 8);
			mapPreview.Material = new ShaderMaterial();
			(mapPreview.Material as ShaderMaterial).Shader = mapSelectedShader.Duplicate() as Shader;
			mapPreviewsContainer.AddChild(mapPreview);

			HBoxContainer playersSelectingContainter = new HBoxContainer();
			playersSelectingContainter.Name = SELECTINGNAME;
			playersSelectingContainter.SetAnchorsPreset(LayoutPreset.TopLeft);
			//playersSelectingContainter.Position += Vector2.Right * 64;
        	playersSelectingContainter.AddThemeConstantOverride("separation", 0);

			mapPreview.AddChild(playersSelectingContainter);

			HBoxContainer playersSelectedContainter = new HBoxContainer();
			playersSelectedContainter.Name = SELECTEDNAME;
			playersSelectedContainter.SetAnchorsPreset(LayoutPreset.BottomRight);
			playersSelectedContainter.Alignment = BoxContainer.AlignmentMode.End;
			playersSelectedContainter.GrowHorizontal = GrowDirection.Begin;
        	playersSelectedContainter.AddThemeConstantOverride("separation", 0);
			playersSelectedContainter.Position += Vector2.Right * 32;
			mapPreview.AddChild(playersSelectedContainter);
        }
		mapPreviewsContainer.ReorganizeMapPreviews();
    }

	ImageTexture GetMapTexturePreview(Map map)
	{
		Vector2I imageSize = new Vector2I(maxMapWidth, maxMapHeight);
        Image image = Image.CreateEmpty(imageSize.X, imageSize.Y, false, Image.Format.Rgba8);
		Color color;
		TileMapLayer tileMap = map.GetNode<TileMapLayer>("InteractableTiles");

		for (int Y = 0; Y < imageSize.Y; Y++)
        {
			for (int X = 0; X < imageSize.X; X++)
			{
				color = White;

				Vector2I tilePos = new Vector2I(X - imageSize.X/2, Y - imageSize.Y/2);
				TileData currentTileData = tileMap.GetCellTileData(tilePos);
				if ( currentTileData != null)
                {
					int tileTerrainIdx = currentTileData.Terrain;
                    color = tileTerrainIdx switch
                    {
                        0 => tilesMapColor["Grass"],
                        1 => tilesMapColor["Factory"],
                        2 => tilesMapColor["Amethyst"],
                        3 => tilesMapColor["Rock"],
                        4 => tilesMapColor["Planks"],
                        5 => tilesMapColor["Glass"],
                        _ => Black,
                    };
                }
				image.SetPixel(X, Y, color);
			}
        }
		foreach(Node2D pickup in map.weaponPickups.GetChildren())
		{
			Vector2 pickupPos = pickup.Position + new Vector2(28f, 17f) * 16;
			pickupPos /= 16f;
			image.SetPixel((int)pickupPos.X, (int)pickupPos.Y -1, Colors.LimeGreen);
		}
		
		return ImageTexture.CreateFromImage(image);
	}

	void OpenMapSelector(bool reverse = false)
    {
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.Out);
		
		if (!reverse)
		{
			tween.TweenMethod(Callable.From((Vector2 wishZoomGiven) =>
			{
				Position = wishZoomGiven;
			}),
			Position, new Vector2(0, 0), 1);
			mapPreviewsContainer.CheckMapPlayerCount();
		}
        else
        {
            tween.TweenMethod(Callable.From((Vector2 wishZoomGiven) =>
			{
				Position = wishZoomGiven;
			}),
			
			Position, new Vector2(0, -(float)ProjectSettings.GetSetting("display/window/size/viewport_height")), 1);
			RemoveVotingPlayers();
			isCommenced = false;
        }
	}

	void RemoveVotingPlayers()
    {
		foreach (Control playerSelector in mapPreviewsContainer.playerSelectors.Values)
        {
            playerSelector.QueueFree();
        }
		foreach (Control playerSelected in mapPreviewsContainer.playerSelecteds.Values)
        {
            playerSelected.QueueFree();
        }

		mapPreviewsContainer.playerSelectors.Clear();
		mapPreviewsContainer.playerSelecteds.Clear();
    }

	public void ChangePlayerMapSelection(Vector2I inputVec, int inputIdx)
    {
		mapPreviewsContainer.ChangePlayerSelection(inputVec, inputIdx);
    }

	public void PlayerAccept(int inputIdx)
    {
        mapPreviewsContainer.PlayerSelectMap(inputIdx);
    }
	
	public void PlayerBack(int inputIdx)
    {
		if (!mapPreviewsContainer.playerSelecteds.ContainsKey(inputIdx)) OpenMapSelector(true);
        mapPreviewsContainer.PlayerDisselectMap(inputIdx);
    }

	public void SelectMap(int selectedMapIdx)
    {
		Map selectedMap = maps[selectedMapIdx];

		Game.Instance.StartGame(playerInfoList, selectedMap);
	}

	Image GetTileImage(TileMapLayer tileMapLayer, Vector2I coords)
    {
        int cellSourceID = tileMapLayer.GetCellSourceId(coords);
		if (cellSourceID == -1) cellSourceID = 1;
        TileSetAtlasSource atlasSource = tileMapLayer.TileSet.GetSource(cellSourceID) as TileSetAtlasSource;
        Vector2I textureCoords = tileMapLayer.GetCellAtlasCoords(coords);
        Rect2I textureRegion = atlasSource.GetTileTextureRegion(textureCoords);
        Image imageTexture = atlasSource.Texture.GetImage();
        return imageTexture.GetRegion(textureRegion);
    }

	Color GetAverageColor(Image image)
    {
		Vector2I imageSize = image.GetSize();
		Vector3 colorVec = new Vector3(0, 0, 0);
	
		for (int Y = 0; Y < imageSize.Y; Y++)
        {
            for (int X = 0; X < imageSize.X; X++)
			{
				Color currentColor = image.GetPixel(X, Y);
				colorVec += new Vector3(currentColor.R, currentColor.G, currentColor.B);
			}
        }
		colorVec /= imageSize.X * imageSize.Y;

		return new Color(colorVec.X, colorVec.Y, colorVec.Z, 1.0f);
    }
}