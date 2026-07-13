using Godot;

[Tool]
public partial class testingGPUparticlesShader : GpuParticles2D
{
    float time = 0;
    public override void _Process(double delta)
    {
        time += (float)delta;

        Position = new Vector2(Mathf.Sin(time) * 100f, 0f); 
    }

}
