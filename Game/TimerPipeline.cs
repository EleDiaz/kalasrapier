using Kalasrapier.Engine;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Graphics.OpenGL;

namespace Kalasrapier.Game;

public class TimerPipeline : RenderPipeline
{
    const string VertexShader = "Shaders/timer_vert.glsl";
    const string FragmentShader = "Shaders/timer_frag.glsl";

    private TimerPawn _timerPawn;

    private int max = 40;

    public TimerPipeline() :
        base(new Shader(VertexShader, FragmentShader))
    {
    }

    public override bool BelongsToPipeline(Actor actor)
    {
        return false;
    }

    public override void Setup(IEnumerable<Actor> actors, Director director)
    {
        _timerPawn = director.ActorManager.FindActorsByTag("Timer").First() as TimerPawn;
    }

    public override void Render(IEnumerable<Actor> actors, Director director)
    {
        GL.FrontFace(FrontFaceDirection.Cw);
        Shader.Use();

        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, (int)(_timerPawn.Percentage * max));
        
        GL.FrontFace(FrontFaceDirection.Ccw);
        Utils.CheckGlError("Draw Timer");
    }
}