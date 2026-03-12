using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace SpaceMages
{
	public partial class SpaceMagesVars : Node
	{
		public static Dictionary<string, Vector3> teamColorsDict
		 = new Dictionary<string, Vector3>()
		 {
			 ["Red"] = new Vector3(0.9f, 0.0f, 0.0f),
			 ["Purple"] = new Vector3(0.8f, 0.0f, 0.8f),
			 ["Blue"] = new Vector3(0.2f, 0.2f, 0.9f),
			 ["Green"] = new Vector3(0.0f, 0.8f, 0.0f),
			 ["Yellow"] = new Vector3(0.8f, 0.8f, 0.0f),
			 ["Orange"] = new Vector3(0.9f, 0.5f, 0.0f),
		 };
		public static Vector3[] teamColors =
		{	
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
	}
}