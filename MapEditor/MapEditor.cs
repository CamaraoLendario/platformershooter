using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Microsoft.VisualBasic;

public partial class MapEditor : Node2D
{
	//debugBegin
	[Export] bool drawDebug = true;
	[Export] Font DebugFont;
	List<Vector2> points = [];
	List<Vector2[]> polygonsForDebug;
	//debugEnd
	[Export] MapTiles currentMap;
	[Export] Camera2D editorCamera;
	TextureRect ghost = new TextureRect();
	Texture2D sheet = GD.Load<Texture2D>("uid://deu7vhg8nqif1");
	Vector2I CurrentTilePos;
	Vector2 prevMousePos;
	MapTiles.Tile currentTile;
	string MapName = "NewUserCreatedMap";
	// TODO: make it so the collision maker cleans up Fives
	List<(Vector2 pos, Vector2I dir, int polyIdx)> fivesToCleanup = [];

	public override void _Ready()
	{
		currentTile = new MapTiles.Tile
		{
			tileType = 0,
			tileOrientation = 15,
		};
		
		updateGhost();

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
		ghost.Position = new Vector2(
			Mathf.Clamp(ghost.Position.X, -32*16 -2, 31*16 -2),
			Mathf.Clamp(ghost.Position.Y, -32*16 -2, 31*16 -2)
		);
		QueueRedraw();
	}

    public override void _Input(InputEvent @event)
    {
		Vector2 currentMousePos = GetGlobalMousePosition();
		Vector2 mouseMovement = currentMousePos - prevMousePos;

		CurrentTilePos = ((Vector2I)ghost.Position + (new Vector2I(33, 33)*16))/16;
		if (Input.IsKeyPressed(Key.Ctrl)){
			if (Input.IsKeyPressed(Key.S)){
				//Stopwatch sw = Stopwatch.StartNew();
				foreach (Node node in currentMap.staticBody.GetChildren())
				{
					node.QueueFree();
				}
				UpdateColision();
				//sw.Stop();
				//GD.Print("generate col elapsed time: " + sw.Elapsed);
			}
		}
		else if (Input.IsKeyPressed(Key.Shift)){
			if (Input.IsMouseButtonPressed(MouseButton.WheelDown)){
				editorCamera.Zoom *= 0.9f;
			}
			else if (Input.IsMouseButtonPressed(MouseButton.WheelUp)){
				editorCamera.Zoom *= 1.1f;
			}
		}
		else
		{
			if (Input.IsMouseButtonPressed(MouseButton.WheelDown)){
				currentTile.tileType = (currentTile.tileType + 1) % 6;
				updateGhost();
			}
			else if (Input.IsMouseButtonPressed(MouseButton.WheelUp)){
				currentTile.tileType = currentTile.tileType - 1;
				if (currentTile.tileType < 0)
					currentTile.tileType = 5;
				updateGhost();
			}
			if (Input.IsMouseButtonPressed(MouseButton.Left)){
				if (currentMap.PlaceBlock(CurrentTilePos, currentTile))
					UpdateColision();
			}
			if (Input.IsMouseButtonPressed(MouseButton.Right)){
				if (currentMap.RemoveBlock(CurrentTilePos))
					UpdateColision();
			}
			if (Input.IsMouseButtonPressed(MouseButton.Middle)){
				editorCamera.Offset -= mouseMovement;
			}
		}
		prevMousePos = GetGlobalMousePosition();
	}

	void updateGhost()
	{
		ghost.Texture = new AtlasTexture()
		{
			Atlas = sheet,
			Region = new Rect2(
				new Vector2(currentTile.tileOrientation % 4, currentTile.tileOrientation / 4) * 20 + new Vector2(currentTile.tileType % 5, currentTile.tileType / 5) * 80,
				new Vector2(20, 20)
			)	
		};
	}

// WIP
/* 	void SaveMap()
	{
		FileAccess file = FileAccess.Open("user://" + MapName + ".bin", FileAccess.ModeFlags.Write);
		
		file.StoreLine(MapName);
		foreach (int idx in currentMap.tileIdxs){
			file.Store16((ushort)idx);
		}

	} */
	void UpdateColision()
	{
		List<Vector2[]> polygons = GenerateCol();
		polygonsForDebug = polygons;
		StaticBody2D polyParent = currentMap.staticBody;
		int polygonsCount = polygons.Count;

		int colPolsCount = polyParent.GetChildCount();

		for(int i = 0; i < polygonsCount; i++)
		{
			if (i < colPolsCount){
				polyParent.GetChild<CollisionPolygon2D>(i).Polygon = polygons[i];
				continue;
			}
			CollisionPolygon2D newPoly = new CollisionPolygon2D{Polygon = polygons[i]};
			polyParent.AddChild(newPoly);
		}

		if (polygonsCount < colPolsCount)
			for(int i = colPolsCount-1; i >= polygonsCount; i--)
				polyParent.GetChild<CollisionPolygon2D>(i).QueueFree();
	
	}

	List<Vector2[]> GenerateCol() {
		points = [];

		List<(int x, int y, int formatIdx)> PBO = []; //PossibleBeginningsOfObjects
		
		for (int y = 0; y < 65; y++){
		for (int x = 0; x < 65; x++){
			int num = GetCrossTilesNum(x, y);
			
			if (!(num == 4 || num == 5)) continue;

			points.Add((new Vector2(x, y) - Vector2.One * 32)*16);
			PBO.Add((x, y, num));
		}}
		
		GD.Print($"PBO Count = {PBO.Count}");
		List<Vector2[]> polygons = [];
		//for(int i = 0; i < PBO.Count; i++)
		int currentPolyIdx = 0;
		while(PBO.Count > 0) {
			GD.Print($"defining polygon {currentPolyIdx}");
			Vector2[] newPoly = DefinePolygon(PBO, 0);

			if (newPoly.Length != 0)
				polygons.Add(newPoly);

			for (int i = fivesToCleanup.Count-1; i >= 0; i--){
				if (fivesToCleanup[i].polyIdx == -1)
					fivesToCleanup[i] = (fivesToCleanup[i].pos, fivesToCleanup[i].dir, currentPolyIdx);
			}
			currentPolyIdx++;
			PBO.RemoveAt(0);
		}

		for(int prnt = 0; prnt < fivesToCleanup.Count; prnt++) {
			GD.Print($"{prnt} - five to cleanup:");
			GD.Print($"pos: {fivesToCleanup[prnt].pos}");
			GD.Print($"dir: {fivesToCleanup[prnt].dir}");
			GD.Print($"idx: {fivesToCleanup[prnt].polyIdx}");
			GD.Print("---------------------");
		}
		// delete fives here maybe?
		while(fivesToCleanup.Count > 0){
			int passedByCount = 0;
			int currentDir;
			List<int> fivesToRemove = [];
			if (fivesToCleanup[0].dir.X == 0)
				currentDir = fivesToCleanup[0].dir.Y;
			else currentDir = fivesToCleanup[0].dir.X;
			for (int i = fivesToCleanup.Count-1; i > 0; i--){
				if (fivesToCleanup[0].pos != fivesToCleanup[i].pos) continue;
				passedByCount++;
				int otherDir;
				if (fivesToCleanup[i].dir.X == 0)
					otherDir = fivesToCleanup[i].dir.Y;
				else otherDir = fivesToCleanup[i].dir.X;
				GD.Print($"currentDir = {currentDir}");
				GD.Print($"otherDir = {otherDir}");
				if (currentDir != otherDir) continue;
				GD.Print("REMOVING");
				polygons.RemoveAt(fivesToCleanup[i].polyIdx);
				//fivesToCleanup.RemoveAt(i);
			}
			fivesToCleanup.RemoveAt(0);
		}

		return polygons;
	}

    private int GetCrossTilesNum(int x, int y)
    {
		int num = 0;
		(int x, int y)[] crossTiles = [
			(-1, -1),
			(0, -1),
			(0, 0),
			(-1, 0),
		];
		(int x, int y) vec = (0, 0);
        for (int i = 0; i < 4; i++){
				vec = crossTiles[i];
				num += Mathf.Clamp(GetIsTile(x + vec.x, y + vec.y), 0, 1) * (int)Mathf.Pow(2, i);
			}
		return num;
    }

	Vector2[] DefinePolygon(List<(int x, int y, int formatIdx)> PBO, int idx) {
        Vector2I originalPos = new Vector2I(PBO[idx].x, PBO[idx].y);
        Vector2I dir = Vector2I.Right;
        Vector2I currentPos = originalPos + dir;
        List<Vector2> polygon = [(originalPos - Vector2.One * 32)*16];
        
        //points.Add((originalPos - Vector2.One * 32)*16);
        int j = 0;
		int rotation = 90;
		GD.Print("current format idx: ", PBO[idx].formatIdx);
		if (PBO[idx].formatIdx == 5)
			fivesToCleanup.Add((new Vector2(originalPos.X, originalPos.Y), -dir, -1));
        while(currentPos != originalPos && j < 100){
            j++;
            
            (int left, int right) LR = GetDirIsTiles(currentPos, dir);
			if (LR.left == 0 && LR.right == 1){
				while(LR.left == 0 && LR.right == 1){
					currentPos += dir;
					LR = GetDirIsTiles(currentPos, dir);
				}
				if (currentPos == originalPos) break;
			}
            List<(int x, int y, int formatIdx)> idxsToRemove = [];
            for (int i = 0; i < PBO.Count; i++)
            {
                if (idx == i) continue;
                if (PBO[i].x == currentPos.X && PBO[i].y == currentPos.Y) {
					int num = GetCrossTilesNum(currentPos.X, currentPos.Y);
					if (num != 5)
						idxsToRemove.Add((currentPos.X, currentPos.Y, num));
					else {
						fivesToCleanup.Add((new Vector2(currentPos.X, currentPos.Y), dir, -1));
						GD.Print("added a five");
					}
				}
            }
            if (LR.left == 1 && LR.right == 1){
				dir = RotateLeft(dir);
                rotation -= 90;
            }
            else if (LR.right == 0){
				dir = RotateRight(dir);
                rotation += 90;
            }
            polygon.Add((currentPos - Vector2.One * 32f)*16f);

            for (int i = idxsToRemove.Count - 1; i >= 0; i--) {
                PBO.Remove(idxsToRemove[i]);
				points.Remove((new Vector2(idxsToRemove[i].x, idxsToRemove[i].y) - Vector2.One * 32)*16);
            }
			//TODO: make it show PBOs when drawing for debugging
            currentPos += dir;
        }
		if (rotation != 360) return [];

        return HolePoly(polygon);
	}

	Vector2[] HolePoly(List<Vector2> polygon){
		//5, 13
		GD.Print("holing...");
		List<Vector2> tileSpacePolygon = [];
		for(int i = 0; i < polygon.Count; i++){
			tileSpacePolygon.Add((polygon[i]/16f) + (Vector2.One*32f));
		}
		float highX = tileSpacePolygon[0].X;
		float highY = tileSpacePolygon[0].Y;
		float lowX = tileSpacePolygon[0].X;
		float lowY = tileSpacePolygon[0].Y;

		List<Vector2I> HolesPoints = [];
		List<List<Vector2>> Holes = [];

		for (int i = 1; i < tileSpacePolygon.Count; i++) {
			highX = Mathf.Max(highX, tileSpacePolygon[i].X);
			highY = Mathf.Max(highY, tileSpacePolygon[i].Y);
			lowX = Mathf.Min(lowX, tileSpacePolygon[i].X);
			lowY = Mathf.Min(lowY, tileSpacePolygon[i].Y);
		}
		int num;
		for(int y = (int)highY - 1; y > lowY; y--){
			for(int x = (int)lowX + 1; x < highX; x++){
				num = GetCrossTilesNum(x, y);
				if (!(num == 5 || num == 13)) continue;
				HolesPoints.Add(new Vector2I(x, y));
			}
		}
		GD.Print($"found {HolesPoints.Count} possible holes");
		for (int i = 0; i < HolesPoints.Count; i++)
		{
			Vector2I originalPos = HolesPoints[i];
			Vector2I dir = Vector2I.Right;
			Vector2I currentPos = originalPos + dir;
			List<Vector2> Hole = [(originalPos - Vector2.One * 32)*16];
			//points.Add((originalPos - Vector2.One * 32)*16);
			int j = 0;
			int rotation = 90;
			while(currentPos != originalPos && j < 100){
				j++;
				(int left, int right) LR = GetDirIsTiles(currentPos, dir);
				if (LR.left == 0 && LR.right == 1){
					while(LR.left == 0 && LR.right == 1){
						currentPos += dir;
						LR = GetDirIsTiles(currentPos, dir);
					}
					if (currentPos == originalPos) break;
				}
				if (LR.left == 1 && LR.right == 1){
					dir = RotateLeft(dir);
					rotation += 90;
				}
				else if (LR.right == 0){
					dir = RotateRight(dir);
					rotation -= 90;
				}
            	Hole.Add((currentPos - Vector2.One * 32f)*16f);
				//points.Add((currentPos - Vector2.One * 32f)*16f);
				
				List<int> idxsToRemove = [];
				for (int I = 0; I < HolesPoints.Count; I++){
					if (i == I) continue;
					if (HolesPoints[I].X == currentPos.X && HolesPoints[I].Y == currentPos.Y){
						idxsToRemove.Add(I);
					}
				}
				for (int I = idxsToRemove.Count - 1; I >= 0; I--){
					HolesPoints.RemoveAt(idxsToRemove[I]);
				}
			}
			GD.Print("Hole rotation: " + rotation);
			if (rotation == 360)
				Holes.Add(Hole);
		}
		GD.Print($"its actually {Holes.Count}");
		for (int h = 0; h < Holes.Count; h++){
			float distSquared = (Holes[h][0] - polygon[0]).LengthSquared();

			int holePoint = 0;
			int otherPoint = 0;

			for (int i = 0; i < Holes[h].Count; i++){
			for (int j = 0; j < polygon.Count; j++){
				float newDistSquared = (Holes[h][i] - polygon[j]).LengthSquared();
				if (newDistSquared < distSquared)
				{
					distSquared = newDistSquared;
					holePoint = i;
					otherPoint = j;
				}
			}}
			// make hole point the first and last point of the hole
			if (holePoint != 0) {
				for (int i = 0; i < holePoint; i++) {
					Holes[h].Add(Holes[h][0]);
					Holes[h].RemoveAt(0);
				}
				Holes[h].Add(Holes[h][0]);
			} 
			else Holes[h].Add(Holes[h][0]);
			// add the hole to the polygon but have the points before and after the hole be:
			// rest of polygon... polygon[j] hole[i], entireHole, hole[i] polygon[j] ...rest of polygon
			polygon.Insert(otherPoint, polygon[otherPoint]);
			polygon.InsertRange(otherPoint+1, Holes[h]);
		}

		return polygon.ToArray();
	}

	(int left, int right) GetDirIsTiles(Vector2I pos, Vector2I dir)
	{
		if (dir == Vector2I.Up)
			return (
				GetIsTile(pos + new Vector2I(-1, -1)),
				GetIsTile(pos + dir)
			);
		
		if (dir == Vector2I.Down)
			return (
				GetIsTile(pos),
				GetIsTile(pos + Vector2I.Left)
			);
		
		if (dir == Vector2I.Left)
			return (
				GetIsTile(pos + dir),
				GetIsTile(pos + new Vector2I(-1, -1))
			);
		
		if (dir == Vector2I.Right)
			return (
				GetIsTile(pos + Vector2I.Up),
				GetIsTile(pos)
			);
		
		GD.Print("GetDirTiles failed. dir not included, returning (pos, pos)");
		return (0, 0);
	}
    private Vector2I RotateLeft(Vector2I dir)
    {
		dir.Y *= -1;
		dir = new Vector2I(-dir.Y, dir.X);
        dir.Y *= -1;
		return dir;
    }
    private Vector2I RotateRight(Vector2I dir)
    {
        return -RotateLeft(dir);
    }

    int GetTile(int idx){
		if (idx < 0 || idx > 4095) return -1;
		return currentMap.tileIdxs[idx];
	}
	int GetTile(Vector2I pos){
		return GetTile(pos.X, pos.Y);
	}
	int GetTile(int x, int y){
		return  GetTile(x + (y*64));
	}
	int GetIsTile(Vector2I vec){
		return GetIsTile(vec.X, vec.Y);
	}
	int GetIsTile(int x, int y){
		return Mathf.Clamp(1 + GetTile(x, y), 0, 1);
	}
    public override void _Draw()
	{
		if (points.Count < 2 || !drawDebug) return;
		DrawPolyline(points.ToArray(), Color.Color8(0, 255, 0), 2);
		for (int i = 0; i < points.Count; i++)
		{
			DrawCircle(points[i], 3, Color.FromHsv(0, 1, 1));
		}
		foreach (Vector2[] polygon in polygonsForDebug)
		for (int i = 0; i < polygon.Length; i++)
		for (int c = 0; c < i.ToString().Length; c++)
			DrawChar(DebugFont, polygon[i]+new Vector2(8*c,0), i.ToString()[c].ToString(), 16, Color.FromHsv(0, 0, 1, 0.75f));
	}
}
