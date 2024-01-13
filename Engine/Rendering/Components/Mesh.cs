using System.Text.Json.Serialization;
using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Components;
using Kalasrapier.Engine.Rendering.Services;
using Kalasrapier.Engine.Rendering.Services.MeshManager;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Components;

public class MeshRef : ComponentData
{
    [JsonPropertyName("mesh_id")]
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
    public MeshData MeshData => MeshManager.GetMesh(MeshId);
    
    public Mesh(Actor actor, string meshId) : base(actor)
    {
        MeshManager = Actor.Director.MeshManager;
        MeshId = meshId;
    }

    public List<Vector3> GetVertexList()
    {
        return MeshManager.GetMesh(MeshId).GetVertexList();
    }

    public void GetVertexDataPerTriangle(out float[] vertexArray, VertexInfo info)
    {
        MeshManager.GetMesh(MeshId).GetVertexDataPerTriangle(out vertexArray, info);
    }
    
    public void GetVertexDataPerVertex(out float[] vertexArray, VertexInfo info)
    {
        MeshManager.GetMesh(MeshId).GetVertexDataPerVertex(out vertexArray, info);
    }

    public void GetIndexArray(out uint[] indexArray)
    {
        MeshManager.GetMesh(MeshId).GetIndexArray(out indexArray);
    }
}
