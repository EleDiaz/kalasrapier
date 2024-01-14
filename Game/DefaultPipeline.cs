using Kalasrapier.Engine;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Components;
using Kalasrapier.Engine.Rendering.Services;
using Kalasrapier.Engine.Rendering.Services.MeshManager;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Kalasrapier.Game;

public class DefaultRenderer : RendererMesh
{
    public DefaultRenderer(Actor actor) : base(actor)
    {
    }
}

public class DefaultPipeline : RenderPipeline
{
    public override VertexInfo VertexInfo => VertexInfo.UV | VertexInfo.NORMALS | VertexInfo.VERTICES;

    const string VertexShader = "Shaders/material_vert.glsl";
    const string FragmentShader = "Shaders/material_frag.glsl";

    private Texture _defaultTexture;
    private DirectionalLight? _directionalLight;
    private Vector3 _defaultColor = new(1, 1, 1);

    public DefaultPipeline() :
        base(new Shader(VertexShader, FragmentShader))
    {
    }

    public override bool BelongsToPipeline(Actor actor)
    {
        return actor.GetComponent<DefaultRenderer>() is not null;
    }

    public override void Setup(IEnumerable<Actor> actors, Director director)
    {
        director.MaterialManager.AddTexture("default", "Textures/default.png");
        director.MaterialManager.LoadTexture("default");
        _defaultTexture = director.MaterialManager.GetTexture("default");


        uint[] indices;
        float[] vertices;
        foreach (var actor in actors)
        {
            var mesh = actor.GetComponent<Mesh>();
            if (mesh is null) {
                Console.WriteLine("Actor: " + actor.Tag + " has no mesh");
                continue;
            }
            var defaultRenderer = new DefaultRenderer(actor);
            
            if (mesh is null) {
                continue;
            }
            mesh.GetVertexDataPerTriangle(out vertices, VertexInfo);
            // This isnt necessary for this case indeed it has a penalty and no pro
            mesh.GetIndexArray(out indices);
            defaultRenderer.LoadMeshDSA(ref vertices, ref indices, VertexInfo);
            defaultRenderer.IndicesLenght = indices.Length;
            defaultRenderer.Slots = mesh?.MeshData?.IndexSlots?.ToArray();
            actor.AddComponent(defaultRenderer);
        }

        _directionalLight = director.ActorManager.GetActors()
            .Select(actor => actor.GetComponent<DirectionalLight>()).First();
    }

    public override void Render(IEnumerable<Actor> actors, Director director)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        var camera = director.Cameras.ActiveCamera;
        if (camera is null || _directionalLight is null)
        {
            return;
        }

        Shader.Use();
        Shader.SetMatrix4("view", camera.GetViewMatrix());
        Shader.SetMatrix4("projection", camera.GetProjectionMatrix());

        Shader.SetVector3("light_direction", _directionalLight.Direction);
        Shader.SetVector3("light_color", _directionalLight.Color);
        // Shader.SetFloat("light_intensity", _directionalLight.Intensity);
        Shader.SetFloat("ambient", 0.1f);

        foreach (var actor in actors)
        {
            var mesh = actor.GetComponent<Mesh>();
            var renderer = actor.GetComponent<DefaultRenderer>();
            var material = actor.GetComponent<Material>();
            if (mesh is null || renderer is null || material is null)
            {
                continue;
            }

            renderer.SetActiveMesh();
            var worldTransform = actor.GetWorldTransform();
            Shader.SetMatrix4("model", worldTransform);
            // https://paroj.github.io/gltut/Illumination/Tut09%20Normal%20Transformation.html
            Shader.SetMatrix4("normal_transform_matrix", Matrix4.Transpose(Matrix4.Invert(worldTransform)));
            
            Shader.SetVector3("diffuse_color", _defaultColor);

            var materialsAmount = Math.Min(material.Slots.Count, renderer.Slots?.Length ?? 0);
            
            for (int i = 0; i < materialsAmount ; i++)
            {
                var indexSlot = renderer!.Slots![i];
                var materialSlot = material.Slots[i];
                if (materialSlot.BaseTexture is not null) {
                    materialSlot.BaseTexture?.Use(TextureUnit.Texture0);
                }
                else {
                    // I'm lazy so let's keep 1 shader for those things
                    _defaultTexture.Use(TextureUnit.Texture0);
                }
                Shader.SetVector3("diffuse_color", materialSlot.DiffuseColor);
                GL.DrawArrays(PrimitiveType.Triangles, (int)indexSlot.Start, (int)indexSlot.Offset);
            }

            if (materialsAmount == 0)
            {
                GL.DrawArrays(PrimitiveType.Triangles, 0, renderer.IndicesLenght);
            }

            Utils.CheckGlError("Draw Mesh: " + mesh.MeshId + " With Actor: " + actor.Tag);
        }
    }
}