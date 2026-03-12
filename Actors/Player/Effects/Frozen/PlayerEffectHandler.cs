using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class PlayerEffectHandler : Node
{
	[Export] private PackedScene iceDDRScene;
	public float PoisonBuildup
    {
        get
        {
            return poisonBuildup;
        }
        set
        {
            poisonBuildup = value;
			(Main.Material as ShaderMaterial).SetShaderParameter("PoisonBuildup", poisonBuildup/timeToPoison);
        }
    }
	public float poisonBuildup = 0;
	public Player poisoner = null;	
	public bool isPoisoned = false;
	public bool isFrozen = false;
	private float timeToPoison = 5;
	private Player Main;
	private PilotAttack pilot;
	private ShipAttack ship;
	private FrozenDDR currentIceDDR;
	
    public override void _Ready()
	{
		Main = GetParent() as Player;
		pilot = Main.pilot;
		ship = Main.ship;
		
		Main.tookDamage += OnDamageTaken;
		Main.reseting += OnReset;

	}

	public override void _PhysicsProcess(double delta)
	{
		ProcessPoison((float)delta);
	}
	void ProcessPoison(float delta)
	{
		if (isPoisoned) PoisonBuildup += delta;
		else PoisonBuildup -= delta / 2;
		if (PoisonBuildup > timeToPoison)
		{
			Main.TakeDamage(poisoner);
			PoisonBuildup = 0;
		}
		if (PoisonBuildup < 0) PoisonBuildup = 0;
	}

	public void Freeze()
	{
		if (!isFrozen)
		{
            currentIceDDR = iceDDRScene.Instantiate<FrozenDDR>();
			
			pilot.inputVector *= 0;
			ship.inputVector *= 0;
            Main.CallDeferred(MethodName.AddChild, currentIceDDR);
		}

		isFrozen = true;
	}
	
	public void UnFreeze()
	{
		isFrozen = false;
    }
	
	void OnReset()
    {
		isPoisoned = false;
		poisoner = null;
		PoisonBuildup = 0;
    }

	
	void OnDamageTaken(Player damageDealer)
	{
		if (isFrozen)
		{
			currentIceDDR.ForceEnd();
		}
	}
}
