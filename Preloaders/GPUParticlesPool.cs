using Godot;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

//TODO: make particlesNodesCount scale with player count
public partial class GPUParticlesPool : Node2D
{
	public static GPUParticlesPool Instance = null;
	static int particleObjectsPerPlayer = 50;
	int particlesNodesCount = 50;
	GpuParticles2D defaultNode = new();
	Node2D droppletsParent;
	Node2D ringsParent;
	GpuParticles2D rings;
	GpuParticles2D dropplets;
	
    public override void _Ready()
    {
		if (Instance == null) Instance = this;
		//Hide();
		ChildEnteredTree += OnNodeEntered;
		ChildExitingTree += OnNodeExited;
        CallDeferred(MethodName.poolParticleEmitters);
    }
	void poolParticleEmitters()
	{
	//defaults
		for(int i = 0; i < particlesNodesCount; i++)
		{
			GenerateNewGeneralUseParticles();
		}

	//IceRay
		IceRay iceRay = GD.Load<PackedScene>("uid://dlbbif2cwel8d").Instantiate<IceRay>();
	//Rings
		ringsParent = new Node2D(){Name = "ringsParent"};
		AddSibling(ringsParent);
		rings = new()
		{
			ProcessMaterial = GD.Load<ParticleProcessMaterial>("uid://bb5cmnqanosm4")	
		};
		SetVariablesTo(rings, iceRay.rayRingEmitter);
		for(int i = 0; i < particlesNodesCount; i++)
		{
			GenerateNewRayRing();
		}
	//Dropplets
		droppletsParent = new Node2D(){Name = "droppletsParent"};
		AddSibling(droppletsParent);
		dropplets = new()
		{
			ProcessMaterial = GD.Load<ParticleProcessMaterial>("uid://um8eg4sda5mp")
		};
		SetVariablesTo(dropplets, iceRay.iceDroppletsEmitter);
		for(int i = 0; i < particlesNodesCount; i++)
		{
			GenerateNewDropplets();
		}
	}

	public static GpuParticles2D GenerateNewDropplets()
	{
		GpuParticles2D newparticles = Instance.dropplets.Duplicate() as GpuParticles2D;
		newparticles.Emitting = false;
		newparticles.ZIndex = 1;
		Instance.droppletsParent.CallDeferred(MethodName.AddChild, newparticles);
		return newparticles;
	}

	public static GpuParticles2D GenerateNewRayRing()
	{
		GpuParticles2D newRings = Instance.rings.Duplicate() as GpuParticles2D;
		newRings.Emitting = false;
		newRings.ZIndex = 1;
		Instance.ringsParent.CallDeferred(MethodName.AddChild, newRings);
		return newRings;
	}

	public static GpuParticles2D GetParticles()
	{
		GpuParticles2D particles = Instance.GetChild<GpuParticles2D>(0);
		if (particles == null)
		{
			GD.PrintErr("More GPUParticles Nodes Necessary, making a new one..");
			particles = GenerateNewGeneralUseParticles();
		}
		Instance.MoveChild(particles, -1);
		//particles.Emitting = true;
		particles.Show();
		return particles;
	}

	static GpuParticles2D GenerateNewGeneralUseParticles()
	{
		GpuParticles2D newParticles = new()
		{
			Emitting = false,
		};
		Instance.CallDeferred(MethodName.AddChild, newParticles);
		return newParticles;
	}

	public static (GpuParticles2D, GpuParticles2D) GetIceParticles()
	{
		(GpuParticles2D, GpuParticles2D) particles = (Instance.ringsParent.GetChild<GpuParticles2D>(0), Instance.droppletsParent.GetChild<GpuParticles2D>(0));
		Instance.ringsParent.MoveChild(particles.Item1, -1);
		Instance.droppletsParent.MoveChild(particles.Item2, -1);
		if (particles.Item1 == null || particles.Item2 == null)
		{
			GD.PrintErr("More IceParticles Nodes Necessary, generating..");
			particles = (GenerateNewDropplets(), GenerateNewRayRing());
		}
		particles.Item1.Emitting = true;
		particles.Item2.Emitting = true;
		return particles;
	}

	public static void NormalizePoolCountToPlayerCount()
	{
		int playerCount = Game.Instance.playerCount;
		int necessaryGeneral = (playerCount - (Instance.GetChildCount() / particleObjectsPerPlayer)) * particleObjectsPerPlayer;
		for (int i = 0; i < necessaryGeneral; i++)
		{
			GenerateNewGeneralUseParticles();
		}
		int necessaryDropplets = (playerCount - (Instance.droppletsParent.GetChildCount() / particleObjectsPerPlayer)) * particleObjectsPerPlayer;
		for (int i = 0; i < necessaryDropplets; i++)
		{
			GenerateNewDropplets();
		}
		int necessaryRings = (playerCount - (Instance.ringsParent.GetChildCount() / particleObjectsPerPlayer)) * particleObjectsPerPlayer;
		for (int i = 0; i < necessaryRings; i++)
		{
			GenerateNewRayRing();
		}
	}

	void DelayedPrintTemp(int str)
	{
		GD.Print(str);
	}

	public static GpuParticles2D GetClonedParticles(GpuParticles2D clonee)
	{
		GpuParticles2D newParticles = GetParticles();
		SetVariablesTo(newParticles, clonee);

		return newParticles;
	}

	public static void SetVariablesToDefault(GpuParticles2D particles)
	{
		SetVariablesTo(particles, Instance.defaultNode);
	}

	public static void SetVariablesTo(GpuParticles2D from, GpuParticles2D to)
	{
		from.Amount = to.Amount;
		from.Texture = to.Texture;
		from.Lifetime = to.Lifetime;
		from.OneShot = to.OneShot;
		from.Preprocess = to.Preprocess;
		from.SpeedScale = to.SpeedScale;
		from.Explosiveness = to.Explosiveness;
		from.FixedFps = to.FixedFps;
		from.LocalCoords = to.LocalCoords;
		from.TrailEnabled = to.TrailEnabled;
		from.ProcessMaterial = to.ProcessMaterial;
		from.Material = to.Material;
		from.ZIndex = GetAbsoluteZindex(to);
		//from.ZAsRelative = false;
	}

	public static void Return(GpuParticles2D particles)
	{
		particles.Reparent(Instance);
		SetVariablesToDefault(particles);
	}
	public static void ReturnRing(GpuParticles2D particles)
	{
		particles.Reparent(Instance.ringsParent);
		particles.Position = Vector2.Zero;
	}
	public static void ReturnDropplet(GpuParticles2D particles)
	{
		particles.Reparent(Instance.droppletsParent);
		particles.Position = Vector2.Zero;
	}

	void OnNodeEntered(Node node)
	{
		if (node is not GpuParticles2D gpuParticles)
		{
			GD.PrintErr("The Node ", node, " is not a GPUParticles2D, this node entered the GPUParticlesPool Node");
			return;
		}
		gpuParticles.Emitting = false;
	}

	void OnNodeExited(Node node)
	{
		if (node is not GpuParticles2D gpuParticles)
		{
			GD.PrintErr("The Node ", node, " is not a GPUParticles2D, this node exited the GPUParticlesPool Node");
			return;
		}
	}

	static int GetAbsoluteZindex(Node2D node)
	{
		int zIndex = 0;

		while (node is Node2D)
		{
			zIndex += node.ZIndex;
			if (!node.ZAsRelative || node.GetParent() is not Node2D)
			{
				break;
			}
			node = node.GetParent<Node2D>();
		}

		return zIndex;
	}

}
