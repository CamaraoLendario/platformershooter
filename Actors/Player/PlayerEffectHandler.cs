using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class PlayerEffectHandler : Node
{
	[Export] CompressedTexture2D iceTexture;
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
	Timer freezeTimer = new();
	private Player Main;
	private PilotAttack pilot;
	private ShipAttack ship;
	private Sprite2D iceSprite;
	private List<PoisonProjectile> poisonBlobs = [];
	
    public override void _Ready()
	{
		Main = GetParent() as Player;
		pilot = Main.pilot;
		ship = Main.ship;
		freezeTimer.OneShot = true;
		AddChild(freezeTimer);
		
		Main.reseting += OnReset;
		pilot.Melee.meleed += OnMeleed;
		ship.dashed += OnShipDashed;
		freezeTimer.Timeout += UnFreeze;
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

	public void Freeze(float freezeTime)
	{
		if (!isFrozen)
		{
            iceSprite = new Sprite2D
            {
                Texture = iceTexture,
                ZIndex = 1
            };
			pilot.inputVector *= 0;
			ship.inputVector *= 0;
            Main.CallDeferred(MethodName.AddChild, iceSprite);
		}

		isFrozen = true;
		freezeTimer.Start(freezeTime);
	}
	
	void UnFreeze()
	{
		isFrozen = false;
		iceSprite.QueueFree();
    }
	
	public void AddPoisonBlob(PoisonProjectile blob)
    {
        poisonBlobs.Add(blob);
		isPoisoned = true;
		poisoner = blob.owner;
    }
	void RemovePoisonBlob(int ammount)
    {
		for(int i = 1; i <= ammount; i++)
		{ 
			if (poisonBlobs.Count <= 0)
			{
				isPoisoned = false;
				return;
			}
			poisonBlobs[0].RemoveFromPlayer();
			poisonBlobs.RemoveAt(0);
		}
    }
	void OnMeleed()
    {
		int removeAmmount = 0;
		if (Main.IsOnFloor())
        {
            removeAmmount = 2;;
        }
		else removeAmmount = 2;
		RemovePoisonBlob(removeAmmount);
    }
	void OnShipDashed()
    {
        RemovePoisonBlob(3);
    }
	
	void OnReset()
    {
        foreach (PoisonProjectile blob in poisonBlobs)
        {
			blob.QueueFree();
        }
		poisonBlobs.Clear();
		isPoisoned = false;
		poisoner = null;
		PoisonBuildup = 0;
    }
}
