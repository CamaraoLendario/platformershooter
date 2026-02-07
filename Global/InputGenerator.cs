using Godot;
using Godot.Collections;
using GodotPlugins.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

public partial class InputGenerator : Node
{
	Array<StringName> actionList = InputMap.GetActions();
	List<StringName> newActions = [];
	public static InputGenerator Instance { get; private set; }

    public override void _Ready()
    {
		Instance = this;
	}

	public void GeneratePlayersInput(List<int> inputIdxs)
	{
		ClearExtraInputs();

		foreach (int inputIdx in inputIdxs)
		{
			foreach (StringName action in actionList)
			{
				string strAction = action.ToString();
				if (strAction.StartsWith("ui_") || strAction.Contains("editor") || strAction.StartsWith("Menu")  || StringEndsWithInt(strAction))
					continue;
				if  (strAction.Contains("Keyboard") && !strAction.EndsWith("Keyboard") || strAction.Contains("Keyboard") && inputIdx != -1)
					continue;

				string currentAction = action + inputIdx;
				InputMap.AddAction(currentAction);
				foreach (InputEvent input in InputMap.ActionGetEvents(action))
				{
					InputEvent currentInput = input.Duplicate(true) as InputEvent;
					currentInput.Device = inputIdx;
					GD.Print(currentAction);
					InputMap.ActionAddEvent(currentAction, currentInput);
					newActions.Add(currentAction);
				}
			}
		}

		actionList = InputMap.GetActions();
	}
	public void ClearExtraInputs(bool includeMenuInput = false)
    {
		var tempNewActions = newActions.ToArray();
		
		foreach(StringName action in tempNewActions)
        {
			GD.Print("Trying to erase Action: " + action);
			if (!includeMenuInput && action.ToString().StartsWith("Menu"))
            {
                GD.Print("not deleting menu actions!");
				continue;
            }
			if (InputMap.HasAction(action))
			{
				InputMap.EraseAction(action);
				GD.Print("Erased Action: " + action);
				newActions.Remove(action);
			}
        }
    }

	public void GeneratePlayersMenuInput(int inputIdx)
	{
		foreach (StringName action in actionList)
		{
			string strAction = action.ToString();
			if (!strAction.StartsWith("Menu") || StringEndsWithInt(strAction))
				continue;
			if ((strAction.Contains("Keyboard") && inputIdx != -1) || (!strAction.Contains("Keyboard") && inputIdx == -1) || (strAction.Contains("Keyboard") && !strAction.EndsWith("Keyboard")))
				continue;
			string currentAction = action + inputIdx;
			InputMap.AddAction(currentAction);
			foreach (InputEvent input in InputMap.ActionGetEvents(action))
			{
				InputEvent currentInput = input.Duplicate(true) as InputEvent;
				currentInput.Device = inputIdx;
				GD.Print(currentAction);
				InputMap.ActionAddEvent(currentAction, currentInput);
				newActions.Add(currentAction);
			}
		}
	}

	public void ClearMenuInput(int inputIdx)
    {
		var tempNewActions = newActions.ToArray();
        foreach (StringName action in tempNewActions)
        {
            string strAction = action.ToString();
			GD.Print("InputIdx of " + inputIdx + " Is trying to erase menu action: " + action);
			int actionInt = GetLastNumOnString(strAction);
			if (strAction.StartsWith("Menu") && actionInt == inputIdx)
            {
                if (InputMap.HasAction(action))
				{
					InputMap.EraseAction(action);
					newActions.Remove(action);
					GD.Print("Erased Action: " + action);
				}
            }
        }
    }

	bool StringEndsWithInt(string str)
    {
		if (str == "") return false;
		
		try
        {
            int Number = str[^1].ToString().ToInt();
			return true;
        }
        catch
        {
            return false;
        }
    }

	int GetLastNumOnString(string inputStr)
	{
		int strLength = inputStr.Length;
		string integerStrReversed = "";
		for(int i = 0; i < strLength; i++)
		{
			Char currentChar = inputStr[strLength - i - 1];
			if (Char.IsNumber(currentChar))
			{
				GD.Print("is number ", currentChar);
				integerStrReversed += currentChar;
			}
			else
			{
				if (currentChar == "-"[0])
				{
					integerStrReversed += currentChar;
				}
				break;
			}
		}
		string integerStr = "";
		for (int i = 0; i < integerStrReversed.Length; i++)
		{
			Char currentChar = integerStrReversed[integerStrReversed.Length - i - 1];
			integerStr += currentChar;
		}
		
		return integerStr.ToInt();
	}
}
