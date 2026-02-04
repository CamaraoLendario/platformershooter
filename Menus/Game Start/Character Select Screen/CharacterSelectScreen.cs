using Godot;
using System;
using SpaceMages;
using System.Collections.Generic;
using System.Linq;
public partial class CharacterSelectScreen : Control
{
	[Export] PackedScene CharacterCapsuleScene;
	[Export] HBoxContainer capsulesContainer;
	[Export] Label gameStartAvaliable;
	[Export] public MapSelector mapSelector;
	public int maxPlayerCount = 6;
	public int playerCount = 0;
	int ReadyPlayerCount
	{
		get
		{
			return readyPlayerCount;
		}
		set
		{
			readyPlayerCount = value;
		}
	}
	int readyPlayerCount = 0;
	Dictionary<int, int> inputDirectory = [];
	public Dictionary<int, bool> avaliableColors = [];
	public Dictionary<int, bool> avaliableNumbers = [];
	

	public override void _Ready()
	{
		inputDirectory.Add(-1, -1);
		for (int i = 0; i <= maxPlayerCount - 1; i++)
		{
			CharacterCapsule newCapsule = CharacterCapsuleScene.Instantiate<CharacterCapsule>();
			newCapsule.Main = this;
			inputDirectory.Add(i, -1);
			newCapsule.changedReadyState += OnCapsuleChangedReadyState;
			capsulesContainer.AddChild(newCapsule);
		}

		avaliableColors.Clear();
		for (int i = 0; i <= SpaceMagesVars.teamColors.Length - 1; i++)
		{
			avaliableColors.Add(i, false);
		}

		avaliableNumbers.Clear();
		for (int i = 1; i <= maxPlayerCount; i++)
		{
			avaliableNumbers.Add(i, false);
		}

		if (Game.Instance.currentPlayerInfoList != null)
		{
			ReconstructPlayerCapsules(Game.Instance.currentPlayerInfoList);
		}
		Input.JoyConnectionChanged += OnJoyConnectionChanged;
	}
	
	private void OnJoyConnectionChanged(long device, bool connected)
    {
		GD.Print("device connection changed: " +  device);
		for (int i = 0; i < Game.Instance.playerNodesByInputIdx.Count; i++)
		{
			int key = Game.Instance.playerNodesByInputIdx.Keys.ToList()[i];
			GD.Print(key, Game.Instance.playerNodesByInputIdx[key].Name);
		}
        if (!connected)
		{
			capsulesContainer.GetChild<CharacterCapsule>(inputDirectory[(int)device]).Leave();
		}
    }

	void ReconstructPlayerCapsules(List<Dictionary<string, int>> playerInfoList)
	{
		// input color capsule
		foreach (Dictionary<string, int> playerInfoDict in playerInfoList)
		{
			EnableCapsule(playerInfoDict.Keys.First(), playerInfoDict["inputIdx"], playerInfoDict["colorIdx"], playerInfoDict["capsuleID"] );
		}
	}

	void OnCapsuleChangedReadyState(bool isReady)
	{
		if (isReady)
			ReadyPlayerCount++;
		else ReadyPlayerCount--;
		CheckReadyStates();
	}
	void CheckReadyStates()
	{
		if (readyPlayerCount == playerCount && playerCount > 0)
			{
				gameStartAvaliable.Modulate = new Color(1, 1, 1, 1);
			}
			else gameStartAvaliable.Modulate = new Color(1, 1, 1, 0);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsReleased() || @event is InputEventMouse || @event is InputEventJoypadMotion) { return; }

		if (IsDeviceNew(@event)) return;
	}

	private void EnableCapsule(string playerName, int deviceIdx, int capsuleID, int colorIdx)
	{
		CharacterCapsule currentCapsule = null;
		foreach (CharacterCapsule characterCapsule in capsulesContainer.GetChildren())
		{
			if (!characterCapsule.isEnabled)
			{
				currentCapsule = characterCapsule;
				break;
			}
		}
		if (currentCapsule == null) return;

		playerCount++;
		inputDirectory[deviceIdx] = playerCount - 1;

		currentCapsule.SetPlayerName(playerName);

		currentCapsule.Enable(deviceIdx, colorIdx);
		GD.Print("I HAVE BEEN REBUILD WITH deviceIdx = " + deviceIdx);

		currentCapsule.capsuleID = capsuleID;
		avaliableNumbers[capsuleID] = true;
		gameStartAvaliable.Modulate = new Color(1, 1, 1, 0);
	}

	private void EnableCapsule(int deviceIdx)
	{
		CharacterCapsule CapsuleAtIndex = capsulesContainer.GetChild(inputDirectory[deviceIdx]) as CharacterCapsule;
		CapsuleAtIndex.Enable(deviceIdx);
		gameStartAvaliable.Modulate = new Color(1, 1, 1, 0);
		for (int i = 1; i <= avaliableNumbers.Count; i++)
		{
			if (!avaliableNumbers[i])
			{
				CapsuleAtIndex.SetPlayerName("P" + i.ToString());
				CapsuleAtIndex.capsuleID = i;
				avaliableNumbers[i] = true;
				break;
			}
		}
	}

	public void OnDisabledCapsule(int deviceIdx)
	{
		playerCount--;

		foreach (int playerNumberKey in inputDirectory.Keys)
		{
			if (inputDirectory[playerNumberKey] != -1 && inputDirectory[playerNumberKey] > inputDirectory[deviceIdx])
			{
				inputDirectory[playerNumberKey]--;
			}
		}

		inputDirectory[deviceIdx] = -1;
		CheckReadyStates();
	}

	private bool IsDeviceNew(InputEvent @event)
	{
		int inputIndex = @event.Device;

		if (!(@event is InputEventJoypadButton || @event is InputEventJoypadMotion))
		{
			inputIndex = -1;
		}

		if (inputDirectory[inputIndex] == -1)
		{
			if (mapSelector.isCommenced) return true;
			if (inputIndex == -1 && playerCount >= maxPlayerCount) return true;
			if (playerCount < maxPlayerCount)
			{
				playerCount++;
				inputDirectory[inputIndex] = playerCount - 1;
				EnableCapsule(inputIndex);
				return true;
			}
			else
			{
				inputDirectory[inputIndex] = inputDirectory[-1];
				inputDirectory[-1] = -1;
				return true;
			}
		}
		GD.Print("Processed input = " + inputIndex);
		foreach (int input in inputDirectory.Values)
		{
			GD.Print(input);
		}
		return false;
	}

	public void PrepareMapSelector()
	{
		if (gameStartAvaliable.Modulate.A != 1) return;
		List<Dictionary<string, int>> playerInfoList = GetPlayerInfoList();
		Game.Instance.currentPlayerInfoList = playerInfoList;
		mapSelector.Commence(playerInfoList);
	}

	List<Dictionary<string, int>> GetPlayerInfoList()
	{
		List<Dictionary<string, int>> playerInfoList = [];

		for (int inputIndex = -1; inputIndex <= inputDirectory.Count - 2; inputIndex++)
		{
			int capsuleIndex = inputDirectory[inputIndex];
			if (capsuleIndex != -1)
			{
				CharacterCapsule currentCapsule = capsulesContainer.GetChild<CharacterCapsule>(capsuleIndex); 
				Dictionary<string, int> playerInfoDict = new Dictionary<string, int>
				{
					[currentCapsule.GetPlayerName()] = -1,
					["inputIdx"] = inputIndex,
					["colorIdx"] = currentCapsule.CurrentColorIdx,
					["capsuleID"] = currentCapsule.capsuleID,
				};
				playerInfoList.Add(playerInfoDict);
				GD.Print("appending ", playerInfoDict);
			}
		}

		return playerInfoList;
	}
}
