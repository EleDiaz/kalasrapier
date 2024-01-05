using System.Text.Json;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using MeshData = Kalasrapier.Engine.ImportJson.MeshData;

namespace Kalasrapier.Engine.Rendering.Services.MeshManager;

public class MeshManager
{
    private Dictionary<string, MeshData> _meshesJson = new();

    /// <summary>
    /// Add json mesh to the manager, so it can be loaded later to the gpu.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="id"></param>
    /// <exception cref="Exception"></exception>
    public void AddMeshResource(string file, string id)
    {
        var meshData = File.ReadAllText(file);

        var mesh = JsonSerializer.Deserialize<MeshData>(meshData);
        if (mesh is null)
        {
            throw new Exception(String.Format("Mesh is null file: {0}, id: {1}", file, id));
        }

        _meshesJson.Add(id, mesh);
    }

    public MeshData GetMesh(string meshId)
    {
        return _meshesJson[meshId];
    }

    // public void LoadMesh(string mesh_id, VertexInfo info)
    // {
    //     var meshJson = _meshesJson[mesh_id];
    //     float[] vertexArray;
    //     uint[] indexArray;
    //     GetVertexArray(meshJson, out vertexArray, info);
    //     GetIndexArray(meshJson, out indexArray);
    //     LoadMeshDSA(mesh_id, ref vertexArray, ref indexArray, info);
    // }

    /*
    public void LoadMaterials(string mesh_id, MeshData meshData)
    {
        var meshInfo = MeshesInfo[mesh_id];
        meshInfo.Slots = meshData.IndexSlots;

        meshInfo.Materials = new Material[meshData.Materials?.Length ?? 0];
        for (int i = 0; i < meshInfo.Materials.Length; i++)
        {
            meshInfo.Materials[i] = new Material(meshData.Materials![i]);
        }
    }
    */
}