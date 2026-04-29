using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Godot;
public partial class CharacterSelectScreen : Control
{
	
}
/* {
	[Export] public MapSelector mapSelector;
	[Export] HBoxContainer playerCapsulesContainer;
	int maxPlayerCount = 6;
	PackedScene capsuleScene = GD.Load<PackedScene>("uid://b7npu7lra7rke");
	List<CharacterCapsule> activatedCapsules = [];
	Dictionary<int, CharacterCapsule> inputDirectory = [];
	public Dictionary<int, bool> colorAvaliability = [];
    public override void _Ready()
    {
        for(int i = 0; i < maxPlayerCount; i++)
		{
			CharacterCapsule newCharacterCapsule = capsuleScene.Instantiate<CharacterCapsule>();
			newCharacterCapsule.Main = this;
			newCharacterCapsule.changedReadyState += CheckPlayersAreReady;
			newCharacterCapsule.wasActivated += OnCapsuleActivated;
			newCharacterCapsule.wasDeactivated += OnCapsuleDeactivated;
			
			playerCapsulesContainer.AddChild(newCharacterCapsule);
		}
    }

	void Rebuld()
	{
		
	}

	void EnableCapsule(int inputIdx)
	{
		foreach (CharacterCapsule characterCapsule in playerCapsulesContainer.GetChildren())
		{
			if (!characterCapsule.isEnabled)
			{
				characterCapsule.Enable(inputIdx);
			}
		}
	}

	void CheckPlayersAreReady()
	{
		if (ArePlayersReady())
		{
			mapSelector.Commence(GetPlayerInfo());
		}
	}
	bool ArePlayersReady()
	{
		foreach (CharacterCapsule capsule in activatedCapsules)
		{
			if (!capsule.IsReady) return false;
		}
		return true;
	}
	void OnCapsuleActivated(CharacterCapsule capsule)
	{
		activatedCapsules.Add(capsule);
	}
	void OnCapsuleDeactivated(CharacterCapsule capsule)
	{
		activatedCapsules.Remove(capsule);
	}
	public void OnDisabledCapsule(CharacterCapsule capsule)
	{
		colorAvaliability[capsule.CurrentColorIdx] = false;
		inputDirectory[capsule.inputNode.inputIdx] = null;
	}

	List<Dictionary<string, int>> GetPlayerInfo()
	{
		List<Dictionary<string, int>> playerInfo = [];

		foreach (CharacterCapsule characterCapsule in activatedCapsules)
		{
			Dictionary<string, int> playerInfoDict = new()
			{
				[characterCapsule.GetPlayerName()] = -1,
				["inputIdx"] = characterCapsule.inputNode.inputIdx,
				["colorIdx"] = characterCapsule.CurrentColorIdx,
			};
			playerInfo.Add(playerInfoDict);
		}

		return playerInfo;
	}

	public void PrintColorAvaliability()
	{
		int num = -1;
		GD.Print("-----------------------\nColor avaliability");
		foreach (bool coloravaliablility in colorAvaliability.Values)
		{
			num ++;
			GD.Print(num, " - ", coloravaliablility);
		}
		GD.Print("-----------------------");
	}

    public int GetPlayerCount()
    {
		return activatedCapsules.Count;
    }

}
 */