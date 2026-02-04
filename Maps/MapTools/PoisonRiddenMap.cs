using Godot;
using System;

public partial class PoisonRiddenMap : Map
{
    Timer poisonCooldownTimer = new Timer();
	float poisonCooldownTime = 20;
	Timer poisonDurationTimer = new Timer();
	float poisonDurationTime = 20;
	bool isMapPoisoned = false;

    public override void _Ready()
    {
        base._Ready();
		SetupTimer(poisonCooldownTimer);
		SetupTimer(poisonDurationTimer);
        CallDeferred(MethodName.connectTimerSignals);
    }

	void SetupTimer(Timer timer)
    {
        timer.OneShot = true;
		AddChild(timer);
    }

    void connectTimerSignals()
    {
        poisonCooldownTimer.Timeout += StartPoison;
        poisonDurationTimer.Timeout += EndPoison; 
		poisonCooldownTimer.Start(poisonCooldownTime);
    }

	void StartPoison()
    {
        isMapPoisoned = true; 
		poisonDurationTimer.Start(poisonCooldownTime);
    }
	
	void EndPoison()
    {
        isMapPoisoned = false;

		foreach((int colorIdx, Player player) in Game.Instance.playerNodesByColor)
        {
			player.effectHandler.isPoisoned = false;
        }

		poisonCooldownTimer.Start(poisonCooldownTime);
    }

    public override void _PhysicsProcess(double delta)
    {
        
		if (isMapPoisoned) ApplyPoison((float) delta);
		base._PhysicsProcess(delta);
    }

	void ApplyPoison(float delta)
    {
        foreach((int colorIdx, Player player) in Game.Instance.playerNodesByColor)
        {
			if (!player.IsInPilotArea) player.effectHandler.isPoisoned = true;
			else player.effectHandler.isPoisoned = false;
        }
    }
}
