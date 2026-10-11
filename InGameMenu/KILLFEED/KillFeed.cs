using Godot;
using System;
using System.Collections.Generic;
using static SpaceMages.SpaceMagesVars;

public partial class KillFeed : Control
{
	public enum WEAPONS {MELEE, ShipProjectile, FIRE, WIND, ICE, SPIRIT, ROCK, TIMEOUTBEAM, MAGICMISSILE, SPIKES}
	const float KILL_FEED_STAY_TIME = 6;
	Texture2D iconsSheet = GD.Load<Texture2D>("uid://dhrf2838cfdr4");
	PackedScene entryTemplate = GD.Load<PackedScene>("uid://coxwxco8e6kp6");
	List <HBoxContainer> killFeedEntries = [];
	float margin = 20;
	Timer scrollTimer = new Timer
	{
		OneShot = true,
		WaitTime = KILL_FEED_STAY_TIME,
	};
    public override void _Ready()
	{
		SignalBus.Instance.playerDied += OnPlayerDead;
		AddChild(scrollTimer);
		scrollTimer.Timeout += ScrollFeed;
	} 

    private async void OnPlayerDead(Player died, Player killer, int weaponUsed)
	{
		HBoxContainer newEntry = GetKillFeedEntry(died, killer, (WEAPONS)weaponUsed);
		killFeedEntries.Add(newEntry);
		GetParent().AddChild(newEntry);
		newEntry.Position = new Vector2(0f, (killFeedEntries.Count-1)*64 + margin);
		Tween tween = CreateTween();
		tween.SetEase(Tween.EaseType.InOut);
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.TweenMethod(Callable.From((float tweenedValue) =>
		{
			newEntry.Position = new Vector2(
				1920-(newEntry.Size.X * tweenedValue) - margin,
				newEntry.Position.Y
			);

		}), 0f, 1f, 0.5f);
		await ToSignal(tween, Tween.SignalName.Finished);
		if (scrollTimer.IsStopped()) scrollTimer.Start();
	}

    HBoxContainer GetKillFeedEntry(Player Victim, Player Killer, WEAPONS weapon)
	{
		HBoxContainer newEntry = entryTemplate.Instantiate() as HBoxContainer;

		(newEntry.GetChild(0) as Label).Text = Killer.NameLabel.Text;
		(newEntry.GetChild(0) as Label).LabelSettings = (newEntry.GetChild(0) as Label).LabelSettings.DuplicateDeep() as LabelSettings;
		(newEntry.GetChild(0) as Label).LabelSettings.FontColor = new Color(
			teamColors[Killer.colorIdx].X, 
			teamColors[Killer.colorIdx].Y, 
			teamColors[Killer.colorIdx].Z
		);
		(newEntry.GetChild(1) as TextureRect).Texture = new AtlasTexture
		{
			Atlas = iconsSheet,
			Region = GetKillFeedIcon(weapon)
		};
		
		(newEntry.GetChild(2) as Label).Text = Victim.NameLabel.Text;
		(newEntry.GetChild(2) as Label).LabelSettings = (newEntry.GetChild(2) as Label).LabelSettings.DuplicateDeep() as LabelSettings;
		(newEntry.GetChild(2) as Label).LabelSettings.FontColor = new Color(
			teamColors[Victim.colorIdx].X, 
			teamColors[Victim.colorIdx].Y, 
			teamColors[Victim.colorIdx].Z
		);

		return newEntry;
	}
	Rect2 GetKillFeedIcon(WEAPONS weapon)
	{
		int Htiles = (int)iconsSheet.GetSize().X/16;
		return new Rect2(
			Position = new Vector2((int)weapon % Htiles, (int)weapon/Htiles) * 16,
			Size = Vector2.One * 16
		);
	}
	async void ScrollFeed()
	{
		Tween tween = CreateTween();
		tween.SetEase(Tween.EaseType.InOut);
		tween.SetTrans(Tween.TransitionType.Sine);
		
		tween.TweenMethod(Callable.From((float tweenedValue) =>
		{
			killFeedEntries[0].Position = new Vector2(
				killFeedEntries[0].Position.X,
				20 - (84 * tweenedValue)
			);
			for(int i = 1; i < killFeedEntries.Count; i++)
			{
				killFeedEntries[i].Position = new Vector2(
					killFeedEntries[i].Position.X,
					((64*i)+20) - (64*tweenedValue)
				);
			}
		}), 0f, 1f, 0.5);
		await ToSignal(tween, Tween.SignalName.Finished);
		killFeedEntries[0].QueueFree();
		killFeedEntries.RemoveAt(0);
		if (killFeedEntries.Count > 0) scrollTimer.Start(KILL_FEED_STAY_TIME - 0.5);
	}
}
