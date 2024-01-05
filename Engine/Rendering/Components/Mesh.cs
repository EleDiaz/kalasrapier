using System.Text.Json.Serialization;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Components;
using Kalasrapier.Engine.Rendering.Services;
using Kalasrapier.Engine.Rendering.Services.MeshManager;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Components;

[JsonDerivedType(typeof(MeshData), typeDiscriminator: "Mesh")]
public class MeshData : ComponentData
{
    public string MeshId { get; set; } = "";
    
    public override Component BuildComponent(Actor actor)
    {
        return new Mesh(actor, MeshId);
    }
}

public class Mesh : Component
{
    public readonly string MeshId;
    private MeshManager MeshManager { get; }
    
    public Mesh(Actor actor, string meshId) : base(actor)
    {
        MeshManager = Actor.Director.MeshManager;
        MeshId = meshId;
    }

    public List<Vector3> GetVertexList()
    {
        return MeshManager.GetMesh(MeshId).GetVertexList();
    }

    public void GetVertexArray(out float[] vertexArray, VertexInfo info)
    {
        MeshManager.GetMesh(MeshId).GetVertexArray(out vertexArray, info);
    }

    public void GetIndexArray(out uint[] indexArray)
    {
        MeshManager.GetMesh(MeshId).GetIndexArray(out indexArray);
    }
}
