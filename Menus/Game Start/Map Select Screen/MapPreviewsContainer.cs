using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public partial class MapPreviewsContainer : Control
{
	[Export] CharacterSelectScreen characterSelectScreen;
	[Export] Control unavaliableMapsNode;
	[Export] AudioStreamPlayer menuBeepPlayer;
	[Export] AnimationPlayer selectMapTimer;
	public MapSelector Main;
	public Dictionary<int, Control> playerSelectors = [];
	public Dictionary<int, Control> playerSelecteds = [];
	List<int> tiedMaps;
	Tween mapRandomizerTween;
	bool everyoneSelected = false;
	int lineLength = 3;
	int lineCount = 0;
	List<TextureRect> mapList = [];

	public void ReorganizeMapPreviews()
	{
		GD.Print("REORGANIZING MAP PREVIEWS...");
		VBoxContainer verticalContainer = new VBoxContainer();
		verticalContainer.Alignment = BoxContainer.AlignmentMode.Center;
		verticalContainer.SetAnchorsPreset(LayoutPreset.FullRect);
		verticalContainer.AddThemeConstantOverride("separation", 30);
		AddChild(verticalContainer);

		HBoxContainer firstHorizontalContainer = new HBoxContainer();
		firstHorizontalContainer.Alignment = BoxContainer.AlignmentMode.Center;
		firstHorizontalContainer.AddThemeConstantOverride("separation", 30);
		verticalContainer.AddChild(firstHorizontalContainer);
		foreach (Node node in GetChildren())
		{
			if (node is not TextureRect mapPreview) continue;
			
			
			HBoxContainer horizontalContainer = verticalContainer.GetChild<HBoxContainer>(-1);
			if (horizontalContainer.GetChildCount() >= lineLength)
			{
				HBoxContainer newHorizontalContainer = new HBoxContainer();
				newHorizontalContainer.Alignment = BoxContainer.AlignmentMode.Center;
				newHorizontalContainer.AddThemeConstantOverride("separation", 30);
				verticalContainer.AddChild(newHorizontalContainer);
				horizontalContainer = newHorizontalContainer;
			}

			mapPreview.Reparent(horizontalContainer);
			mapList.Add(mapPreview);
		}
		lineCount = GetChild(0).GetChildCount();
		GD.Print("Line Count gotten = " + lineCount);
	}

	public void CheckMapPlayerCount()
	{
		foreach(TextureRect mapPreview in unavaliableMapsNode.GetChildren())
		{
			mapPreview.Reparent(GetChild(0).GetChild(lineCount-1));
			mapList.Add(mapPreview);
		}
		
		var tempMapList = mapList.ToList();
		foreach(TextureRect mapPreview in tempMapList)
		{
			int mapMaxPlayerCount = Main.maps[tempMapList.IndexOf(mapPreview)].GetNode("SpawnPoints").GetChildCount();
			GD.Print(mapMaxPlayerCount);
			if ( mapMaxPlayerCount >= characterSelectScreen.playerCount)
				continue;

			mapList.Remove(mapPreview);
			mapPreview.Reparent(unavaliableMapsNode);
		}

		var newtempMapList = mapList.ToList();
		int mapNumber = 0;
		foreach(TextureRect mapPreview in newtempMapList)
		{
			int lineNumber = mapNumber/lineLength;
			mapPreview.Reparent(GetChild(0).GetChild(lineNumber));
			mapNumber ++;
		}
	}

	public void AddMapSelector(Control newMapSelector, int playerInputIdx, int mapPreviewIdx = 0)
	{
		TextureRect mapPreview = GetMapPreview(mapPreviewIdx);
		Node selectingNode = mapPreview.GetNode(MapSelector.SELECTINGNAME);
		selectingNode.AddChild(newMapSelector);

		playerSelectors.Add(playerInputIdx, newMapSelector);
	}

	private TextureRect GetMapPreview(int mapPreviewIdx)
	{
		if (mapPreviewIdx < 0)
		{
			mapPreviewIdx = mapList.Count + mapPreviewIdx;
			
		}
		GD.Print("New map preview Idx: " + mapPreviewIdx);

		return mapList[mapPreviewIdx];
	}
	TextureRect GetMapPreview(Vector2I mapPreviewVec)
	{
		if (mapPreviewVec.Y < 0)
		{
			mapPreviewVec.Y = lineCount - 1;
		}
		else if (mapPreviewVec.Y >= lineCount)
		{
			mapPreviewVec.Y = 0;
		}

		if (mapPreviewVec.X + mapPreviewVec.Y * lineLength >= mapList.Count)
		{
			mapPreviewVec.X = -1;
		}

		GD.Print("NEW MAP POS " + mapPreviewVec);
		return GetChild(0).GetChild(mapPreviewVec.Y).GetChild(mapPreviewVec.X) as TextureRect;
	}
	Vector2I getMapPreviewPos(TextureRect mapPreview)
	{
		int mapIdx = mapList.IndexOf(mapPreview);
		int X = mapIdx % lineLength;
		int Y = mapIdx/lineLength;
		return new Vector2I(X, Y);
	}

	public void ChangePlayerSelection(Vector2I inputVec, int inputIdx)
	{
		if (CheckSelectorsIdx(inputIdx)) return;

		TextureRect mapPreview = playerSelectors[inputIdx].GetParent().GetParent() as TextureRect;
		int currentSelectedPreview = mapList.IndexOf(mapPreview);
		if (inputVec.Y == 0)
		{
			currentSelectedPreview += inputVec.X;
		}
		else
		{
			Vector2I mapPos = getMapPreviewPos(mapPreview);
			GD.Print("GOT MAP POS " + mapPos);
			mapPos += inputVec;
			GD.Print("NEW MAP POS INPUT " + mapPos);
			currentSelectedPreview = mapList.IndexOf(GetMapPreview(mapPos));
		}

		if (currentSelectedPreview > mapList.Count - 1) currentSelectedPreview = 0;

		SetMapSelectorPreview(playerSelectors[inputIdx], currentSelectedPreview);
	}

	void SetMapSelectorPreview(Control playerSelector, int newPos)
	{
		ReparentSelector(playerSelector, GetMapPreview(newPos).GetNode<Control>(MapSelector.SELECTINGNAME));
	}

	void ReparentSelector(Control playerSelectorNode, Control Target, int offsetFactor = 1, float targetScale = 4.0f)
	{
		Vector2 initialGlobalPosition = playerSelectorNode.GetChild<Sprite2D>(0).GlobalPosition;
		Control dummyNode = playerSelectorNode.Duplicate() as Control;
		
		Sprite2D dummySprite = dummyNode.GetChild<Sprite2D>(0);
		dummyNode.CustomMinimumSize = new Vector2(16 * dummySprite.Scale.X, 0);
		dummySprite.QueueFree();
		
		playerSelectorNode.AddSibling(dummyNode);
		playerSelectorNode.Reparent(Target);
		playerSelectorNode.CustomMinimumSize = new Vector2(0, playerSelectorNode.CustomMinimumSize.Y);
		CallDeferred(MethodName.tweenSelectorPos, playerSelectorNode, dummyNode, initialGlobalPosition, offsetFactor, targetScale);
	}
	void tweenSelectorPos(Control playerSelectorNode, Control dummyNode, Vector2 initialGlobalPosition, int offsetMultiplier, float targetScale)
	{
		Sprite2D playerSelectorSprite = playerSelectorNode.GetChild<Sprite2D>(0);
		playerSelectorSprite.GlobalPosition = initialGlobalPosition;
		Tween tween = CreateSineEasedTween();
		Vector2 initialPosition = playerSelectorSprite.Position;
		float initialScale = playerSelectorSprite.Scale.X;
		Vector2 textureSize = playerSelectorSprite.Texture.GetSize();
		Vector2 offsetFactor = textureSize / 2;
		Vector2 initialOffset = playerSelectorSprite.Offset;
		Vector2 targetOffset = offsetFactor * offsetMultiplier;
		float targetCustomMinimumSizeX = 16 * targetScale;
		float initialDummyCustomMinimumSizeX = dummyNode.CustomMinimumSize.X;

		GD.Print(initialScale);

		tween.TweenMethod(Callable.From((float tweenFactor) =>
		{
			playerSelectorSprite.Position = initialPosition * tweenFactor;
			playerSelectorSprite.Offset = initialOffset + ((targetOffset - initialOffset) * (1 - tweenFactor));
			playerSelectorSprite.Scale = (initialScale + ((targetScale - initialScale) * (1 - tweenFactor))) * Vector2.One;
			playerSelectorNode.CustomMinimumSize = new Vector2(targetCustomMinimumSizeX * (1 - tweenFactor), 0);
			dummyNode.CustomMinimumSize = new Vector2(initialDummyCustomMinimumSizeX - initialDummyCustomMinimumSizeX * (1 - tweenFactor), 0);
		}
		), 1.0f, 0.0f, 0.1f);
		tween.Finished += () =>
		{
			dummyNode.QueueFree();
		};
	}
	Tween CreateSineEasedTween()
	{
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.InOut);
		return tween;
	}

	public void PlayerSelectMap(int inputIdx)
	{
		if (CheckSelectorsIdx(inputIdx)) return;

		GD.Print(inputIdx + " Has selected a map");

		Control currentPlayerSelector = playerSelectors[inputIdx];
		HBoxContainer selectedContainer = currentPlayerSelector.GetParent().GetParent().GetNode<HBoxContainer>(MapSelector.SELECTEDNAME);
		ReparentSelector(currentPlayerSelector, selectedContainer, -1, 2f);

		playerSelectors.Remove(inputIdx);
		playerSelecteds.Add(inputIdx, currentPlayerSelector);

		CheckMapVotes();
	}

	bool CheckSelectorsIdx(int inputIdx)
	{
		if (!playerSelectors.ContainsKey(inputIdx)) 
			{GD.PrintErr("The requested inputIdx was not found");   return true;}
		if (!playerSelectors[inputIdx].Visible)
			{GD.Print("The requested inputIdx has already selected a map");   return true;}

		return false;
	}

	public void PlayerDisselectMap(int inputIdx)
	{
		if (!playerSelecteds.ContainsKey(inputIdx)) return;

		GD.Print(inputIdx + " Has disselected a map");
		Control currentPlayerSelector = playerSelecteds[inputIdx];

		HBoxContainer selectedContainer = currentPlayerSelector.GetParent().GetParent().GetNode<HBoxContainer>(MapSelector.SELECTINGNAME);	
		
		ReparentSelector(currentPlayerSelector, selectedContainer);
		playerSelecteds.Remove(inputIdx);
		playerSelectors.Add(inputIdx, currentPlayerSelector);

		selectMapTimer.Stop();
		selectMapTimer.Play("RESET");
		everyoneSelected = false;
		
		if (mapRandomizerTween != null)
		{
			mapRandomizerTween.Stop();
			foreach(TextureRect mapPreview in mapList)
			{
				(mapPreview.Material as ShaderMaterial).SetShaderParameter("Thickness", 0);
			}
		}
	}

	void CheckMapVotes()
	{
		foreach(Control selector in playerSelectors.Values)
		{
			if (selector.Visible) return;
		}

		everyoneSelected = true;
		List<int> tiedMaps = [];
		int selectedMapIdx = 0;
		int currentVotesRecord = -1;
		
		tiedMaps.Clear();

		foreach (TextureRect map in mapList)
		{
			int votesCount = map.GetNode(MapSelector.SELECTEDNAME).GetChildCount();

			if (currentVotesRecord < votesCount)
			{
				tiedMaps.Clear();
				selectedMapIdx = mapList.IndexOf(map);
				currentVotesRecord = votesCount;
			}
			else if (currentVotesRecord == votesCount)
			{
				tiedMaps.Add(mapList.IndexOf(map));
			}
		}

		if (tiedMaps.Count != 0)
		{
			tiedMaps.Add(selectedMapIdx);
			this.tiedMaps = tiedMaps;
			AnimateMapRandomizer();
		}
		else
			SelectMap(selectedMapIdx);
	}

	void AnimateMapRandomizer()
	{	
		mapRandomizerTween = CreateTween();
		mapRandomizerTween.SetTrans(Tween.TransitionType.Circ);
		mapRandomizerTween.SetEase(Tween.EaseType.Out);

		int endDistance = 30 + GD.RandRange(0, tiedMaps.Count-1);
		
		mapRandomizerTween.TweenMethod(Callable.From<int>(ProcessMapRandomizerSelector), 0, endDistance, 3);
		
		mapRandomizerTween.Finished += () =>
		{
			int tiedMapsCount = tiedMaps.Count;
			
			endDistance %= tiedMapsCount;

			Main.SelectMap(tiedMaps[endDistance]);
		};
	}
	int currentTiedMapsIdx = -1;
	void ProcessMapRandomizerSelector(int selectorDistance)
	{
		if (!everyoneSelected)
		{
			return;
		}
		if (currentTiedMapsIdx == selectorDistance && currentTiedMapsIdx != -1) return;
		currentTiedMapsIdx = selectorDistance;
		
		menuBeepPlayer.Play();
		int tiedMapsCount = tiedMaps.Count;
		selectorDistance %= tiedMapsCount;

		(mapList[tiedMaps[selectorDistance]].Material as ShaderMaterial).SetShaderParameter("Thickness", 1);
		selectorDistance -= 1;
		if (selectorDistance < 0)
		{
			selectorDistance = tiedMapsCount - 1;
		}
		(mapList[tiedMaps[selectorDistance]].Material as ShaderMaterial).SetShaderParameter("Thickness", 0);
	}

	void SelectMap(int mapIdx)
	{
		selectMapTimer.Play("mapSelectTimer");
        selectMapTimer.AnimationFinished += (StringName animName) =>
		{
			if (animName != "mapSelectTimer") return;
			
			Main.SelectMap(mapIdx); 
		};
	}
}