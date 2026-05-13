using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace SpaceMages
{
	public partial class SpaceMagesVars : Node
	{
		enum dirKeyMenu{
			UpKeyMen,
			DownKeyMen,
			RightKeyMen,
			LeftKeyMen,
		}
		public static string[] menuDirs = [
			"MenuLeft", "MenuRight", "MenuUp", "MenuDown"];
		public static Dictionary<string, Vector3> teamColorsDict = new Dictionary<string, Vector3>(){
			["Red"] = new Vector3(0.9f, 0.0f, 0.0f),
			["Purple"] = new Vector3(0.8f, 0.0f, 0.8f),
			["Blue"] = new Vector3(0.2f, 0.2f, 0.9f),
			["Green"] = new Vector3(0.0f, 0.8f, 0.0f),
			["Yellow"] = new Vector3(0.8f, 0.8f, 0.0f),
			["Orange"] = new Vector3(0.9f, 0.5f, 0.0f),
		};
		public static Vector3[] teamColors = {	
			teamColorsDict["Red"],
			teamColorsDict["Purple"],
			teamColorsDict["Blue"],
			teamColorsDict["Green"],
			teamColorsDict["Yellow"],
			teamColorsDict["Orange"],
		};

		public static List<Vector2> checkDirections = [
			Vector2.Right,
			Vector2.Down,
			Vector2.Left,
			Vector2.Up,
		];
		public static bool StringEndsWithInt(string str)
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
		public static (string main, string inputIdx) GetStringAndInt(string givenAction)
		{
			if (givenAction == "")
			{
				GD.PrintErr("givenAction is empty!");
				return (givenAction, "");
			}
			(string main, string inputIdx) segments = (givenAction, "");
			if (StringEndsWithInt(givenAction))
			{
				string intlessString = givenAction;
				while(StringEndsWithInt(intlessString) || intlessString[^1] == "-"[0]){
					if (segments.inputIdx.Length > 0)
						segments.inputIdx = segments.inputIdx.Insert(0, intlessString[^1].ToString());
					else segments.inputIdx += intlessString[^1].ToString();
					intlessString = intlessString.Remove(NormalizeIdx(-1, intlessString.Length));
				}
				segments.main = intlessString;
			}
			return segments;
		}
		public static int NormalizeIdx(int idx, int collectionSize)
		{
			if (collectionSize == 0)
			{
				GD.PrintErr("Tried to normalize Idx of a collection with size 0, returning 0");
				return 0;
			}

			if (idx < 0)
			{
				return (collectionSize - (-idx % collectionSize))% collectionSize;
			}
			if (idx >= collectionSize)
			{
				return idx % collectionSize;
			}
			return idx;
		}

		public static Vector2 GetScreenRez()
		{
			return new Vector2(
			(float) ProjectSettings.GetSetting("display/window/size/viewport_width"),
			(float) ProjectSettings.GetSetting("display/window/size/viewport_height")
			);
		}
	}
}