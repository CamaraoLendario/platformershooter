using Godot;
using SpaceMages;
using System;
using System.Net.Mail;
using System.Text.Encodings.Web;

public partial class MapCamera : Camera2D
{
	float wishZoom = 0;
	public float boardersLeeway;
	Vector2 PixelMapSize;
	Vector2 screenSize = new Vector2(
		(float)ProjectSettings.GetSetting("display/window/size/viewport_width"),
		(float)ProjectSettings.GetSetting("display/window/size/viewport_height")
	); //REMEMBER TO SET A SIGNAL TO CHANGE THIS WHENEVER THE SCREEN REZ CHANGESAAAAA
	float shakeForce = 0f;
	float shakeTime = 0.2f;
	float minimumScreenSize = 448; // 28 tiles accross
	float baseMinZoom;
	float maxZoom = 1;
	float minZoom = 1;
	Vector2 targetPos = Vector2.Zero;
 
    public override void _Ready()
	{
		if (Engine.IsEditorHint()) return;
		
		Game.Instance.playerDied += OnPlayerDied;

		PixelMapSize = GetParent<Map>().PixelsMapSize;
        baseMinZoom = base.Zoom.X;
		maxZoom = 1 / (minimumScreenSize / screenSize.X);
		GD.Print("maxZoom = " + maxZoom);
    }

	public void SetLeeway(int leeway)
    {
        boardersLeeway = leeway;
		PixelMapSize += Vector2.One * boardersLeeway;
		minZoom = baseMinZoom - (boardersLeeway * 2)/screenSize.Y;
		GD.Print("minZoom = " + minZoom);
    }

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.IsEditorHint()) return;
		//screenSize = DisplayServer.WindowGetSize();
		targetPos = GetCenter();
		SetZoom();
		ProcessZoom();

		targetPos = StayWithinBounds();
		EasePos();
		
		if (shakeForce > 0) ProcessCameraShake();
        RenderingServer.GlobalShaderParameterSet("cameraDistFromOrigin", GlobalPosition);
	}




	Vector2 StayWithinBounds()
	{
		if (targetPos.X + ((screenSize.X / 2) / base.Zoom.X) > (PixelMapSize.X) / 2)
		{
			targetPos = new Vector2(PixelMapSize.X / 2 - ((screenSize.X / 2) / base.Zoom.X), targetPos.Y);
		}
		if (targetPos.X - ((screenSize.X / 2) / base.Zoom.X) < -(PixelMapSize.X) / 2)
		{
			targetPos = new Vector2(-PixelMapSize.X / 2 + ((screenSize.X / 2) / base.Zoom.X), targetPos.Y);
		}
		if (targetPos.Y + ((screenSize.Y / 2) / base.Zoom.X) > (PixelMapSize.Y) / 2)
		{
			targetPos = new Vector2(targetPos.X, PixelMapSize.Y / 2 - ((screenSize.Y / 2) / base.Zoom.X));
		}
		if (targetPos.Y - ((screenSize.Y / 2) / base.Zoom.X) < -(PixelMapSize.Y) / 2)
		{
			targetPos = new Vector2(targetPos.X, -PixelMapSize.Y / 2 + ((screenSize.Y / 2) / base.Zoom.X));
		}

		return targetPos;
    }

	Vector2 GetCenter()
	{
		Vector2 medianPos = Vector2.Zero;

		foreach (Player player in Game.Instance.playerNodesByColor.Values)
		{
			if (player.IsDead) continue;
			medianPos += player.GlobalPosition / Game.Instance.alivePlayerCount;
		}

		return medianPos;
	}

	void SetZoom()
	{
		Vector2 CurrentLengthVector = Vector2.One * 0;
		float currentGreatestLength = 0;
		foreach (Player playerA in Game.Instance.playerNodesByColor.Values)
		{
			foreach (Player playerB in Game.Instance.playerNodesByColor.Values)
			{
				if (playerB == playerA) continue;

				Vector2 playerpos1 = new Vector2(playerA.Position.X, playerA.Position.Y);
				Vector2 playerpos2 = new Vector2(playerB.Position.X, playerB.Position.Y);

				float currentLengthX = Mathf.Abs(playerpos1.X - playerpos2.X);
				float currentLengthY = Mathf.Abs(playerpos1.Y - playerpos2.Y) * (screenSize.X / screenSize.Y);

				if (currentGreatestLength < currentLengthX || currentGreatestLength < currentLengthY)
                {
                    CurrentLengthVector = new Vector2(currentLengthX, currentLengthY);
					if (currentLengthX > currentLengthY) currentGreatestLength = currentLengthX;
					else currentGreatestLength = currentLengthY;
                }
			}
		}

		if (CurrentLengthVector.X > CurrentLengthVector.Y * (screenSize.X/screenSize.Y))
        {
			wishZoom = 1 / (currentGreatestLength / screenSize.X);
        }
        else
        {
            wishZoom = 1 / (currentGreatestLength /(screenSize.X/screenSize.Y) / screenSize.Y);
        }

		wishZoom /= 2f;

		if (wishZoom < minZoom) wishZoom = minZoom;
		else if (wishZoom > maxZoom) wishZoom = maxZoom;
	}

	void ProcessZoom()	
    {
		/* Vector2 zoomDir = (new Vector2(wishZoom, wishZoom) - Zoom).Normalized();
		
		Zoom += zoomDir * ZoomSpeed * (float)GetProcessDeltaTime();*/

		Tween tween = CreateTween();

		tween.TweenMethod(Callable.From((Vector2 wishZoomGiven) =>
        {
            Zoom = wishZoomGiven;
        }),
		Zoom, new Vector2(wishZoom, wishZoom), 0.5f);
    }

	void ProcessCameraShake()
	{
		Vector2 randomVector = new Vector2(GD.RandRange(-1, 1), GD.RandRange(-1, 1)).Normalized();
		Offset = randomVector * shakeForce;
		shakeForce -= (float) (GetPhysicsProcessDeltaTime()/shakeTime) * 15f;
		if (shakeForce < 0) shakeForce = 0;
	}

	void OnPlayerDied(Player died, Player killed)
	{
		if (Game.Instance.alivePlayerCount == 1)
		{
			shakeForce = 15;
			shakeTime = 1;
		}
        else
        {
            shakeForce = 15f;
			shakeTime = 0.3f;
        }
		Game.Instance.ApplyHitstop(shakeTime*0.6f);
	}

	void EasePos()
	{
		Vector2 vecDiff = targetPos - Position;
		float distanceSquared = vecDiff.LengthSquared();

		if (distanceSquared < 1 / Zoom.X)
		{
			Position = targetPos;
			return;
		}
		float step = ((float) GetProcessDeltaTime() * vecDiff * 5).Length();


		Position += vecDiff.Normalized() * step;
	}
}
