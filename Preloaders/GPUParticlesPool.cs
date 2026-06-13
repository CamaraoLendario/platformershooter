using Godot;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

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

	public static void NormalizePoolCountToPlayerCount()
	{
		int playerCount = Game.Instance.players.Length;
		int necessaryGeneral = (playerCount - (Instance.GetChildCount() / particleObjectsPerPlayer)) * particleObjectsPerPlayer;
		for (int i = 0; i < necessaryGeneral; i++)
		{
			GenerateNewGeneralUseParticles();
		}
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
		from.ZAsRelative = to.ZAsRelative;
		from.ProcessMode = to.ProcessMode;
	}

	public static void Return(GpuParticles2D particles)
	{
		particles.Reparent(Instance);
		SetVariablesToDefault(particles);
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
