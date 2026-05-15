using Godot;
using System;

public partial class Gamemode : Node
{
	GamemodeLogic logic = new FreeForAllLogic();
   public override void _Ready()
    {
        SignalBus.Instance.GameStarted += logic.OnGameStarted;
		SignalBus.Instance.playerDied += logic.OnPlayerDied;
    }

}
