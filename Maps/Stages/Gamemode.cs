using Godot;
using System;

public partial class Gamemode : Node
{
	GamemodeLogic logic;
   public override void _Ready()
    {
        logic = Game.GetGamemodeLogic();
        GD.Print("node is ready!");
        SignalBus.Instance.GameStarted += logic.OnGameStarted;
		SignalBus.Instance.playerDied += logic.OnPlayerDied;
        SignalBus.Instance.FinishedSpawningPlayers += logic.OnFinishedSpawningPlayers;
    }

    public override void _ExitTree()
    {
        SignalBus.Instance.GameStarted -= logic.OnGameStarted;
		SignalBus.Instance.playerDied -= logic.OnPlayerDied;
        SignalBus.Instance.FinishedSpawningPlayers -= logic.OnFinishedSpawningPlayers;
    } 
}
