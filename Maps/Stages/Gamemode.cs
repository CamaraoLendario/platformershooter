using Godot;
using System;

public partial class Gamemode : Node
{
	GamemodeLogic logic;
   public override void _Ready()
    {
        logic = Game.GetGamemodeLogic();
        SignalBus.Instance.GameStarted += logic.OnGameStarted;
		SignalBus.Instance.playerDied += logic.OnPlayerDied;
    }


}
