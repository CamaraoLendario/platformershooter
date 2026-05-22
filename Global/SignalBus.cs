using Godot;
using System;
using System.ComponentModel;

public partial class SignalBus : Node
{
	[Signal] public delegate void playerDiedEventHandler(Player died, Player killer);
	[Signal] public delegate void PlayerAddedEventHandler(Player player);
	[Signal] public delegate void FinishedSpawningPlayersEventHandler();	
	[Signal] public delegate void GameStartedEventHandler();
	[Signal] public delegate void GameFinishedEventHandler(int winningColorIdx);
	[Signal] public delegate void NewRoundStartEventHandler();
	[Signal] public delegate void RoundFinishedEventHandler();	
	[Signal] public delegate void PauseRequestEventHandler(Player player, bool pausedByDisconnect);
	
	public static SignalBus Instance { get; private set; }
    public override void _Ready()
	{
		Instance ??= this;
	}
	static public Variant Emit(string signal)
	{
		SignalBus.Instance.EmitSignal(signal);
		return 1;
	}
}
