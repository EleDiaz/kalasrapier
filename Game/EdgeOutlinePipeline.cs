using Kalasrapier.Engine;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Components;
using Kalasrapier.Engine.Rendering.Services;
using Kalasrapier.Engine.Rendering.Services.MeshManager;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Kalasrapier.Game;

public class OutlineRenderer : RendererMesh
{
    public OutlineRenderer(Actor actor) : base(actor)
    {
    }
}

public class EdgeOutlinePipeline : RenderPipeline
{
    public override VertexInfo VertexInfo => VertexInfo.VERTICES;

    const string VertexShader = "Shaders/Outline_vert.glsl";
    const string FragmentShader = "Shaders/Outline_frag.glsl";


    public EdgeOutlinePipeline() :
        base(new Shader(VertexShader, FragmentShader))
    {
    }

    public override void Setup(IEnumerable<Actor> _actors, Director director)
    {
        var actors = director.ActorManager.FindActorsByTag("pawn");
        // I doubt that this is giving any valuable performance.
        uint[] indices;
        float[] vertices;
        
        foreach (var actor in actors)
        {
            var renderer = new OutlineRenderer(actor);
            actor.AddComponent(renderer);
            var mesh = actor.GetComponent<Mesh>();
            mesh.GetIndexArray(out indices);
            mesh.GetVertexArray(out vertices, VertexInfo);
            renderer.LoadMeshDSA(ref vertices, ref indices, VertexInfo);
        }
    }

    public override void Render(IEnumerable<Actor> actors, Director director)
    {
        // We will render the backfaces
        GL.FrontFace(FrontFaceDirection.Cw);
        var camera = director.Cameras.ActiveCamera;
        if (camera is null)
        {
            return;
        }

        Shader.Use();
        Shader.SetMatrix4("view", camera.GetViewMatrix());
        Shader.SetMatrix4("projection", camera.GetProjectionMatrix());

        foreach (var actor in actors)
        {
            var mesh = actor.GetComponent<Mesh>();
            var rendererMesh = actor.GetComponent<OutlineRenderer>();
            if (mesh is null || rendererMesh is null)
            {
                continue;
            }

            rendererMesh.SetActiveMesh();
            var transform = actor.GetWorldTransform();
            // Higher the scale higher the outline
            transform *= Matrix4.CreateScale(1.1f);
            Shader.SetMatrix4("model", transform);


            // https://docs.gl/gl4/glDrawElements
            GL.DrawElements(PrimitiveType.Triangles, rendererMesh.IndicesLenght, DrawElementsType.UnsignedInt, 0);

            Utils.CheckGlError("Draw Mesh: " + mesh.MeshId + " With Actor: " + actor.Tag);
        }

        // get back the default behavior
        GL.FrontFace(FrontFaceDirection.Ccw);
    }
}
