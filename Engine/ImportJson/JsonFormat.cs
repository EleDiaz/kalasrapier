using System.Text.Json.Serialization;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Components;
using Kalasrapier.Engine.Rendering.Services.MeshManager;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.ImportJson;

public record ActorData
{
    [JsonPropertyName("tag")] public required string Tag { get; init; }
    [JsonPropertyName("enabled")] public bool Enabled { get; init; } = true;
    [JsonPropertyName("position")] public float[] Position { get; init; } = { 0, 0, 0 };
    [JsonPropertyName("scale")] public float[] Scale { get; init; } = { 1, 1, 1 };
    [JsonPropertyName("orientation")] public OrientationData Orientation { get; init; } = new();
    [JsonPropertyName("children")] public List<ActorData> Children { get; init; } = new();
    [JsonPropertyName("components")] public List<ComponentData> Components { get; init; } = new();
}

public record OrientationData
{
    [JsonPropertyName("axis")] public float[] Axis { get; init; } = { 0, 1f, 0 };
    [JsonPropertyName("angle")] public float Angle { get; init; } = 0;
}

public record MeshData
{
    // 3 floats
    [JsonPropertyName("vertex_data")] public required float[] VertexData { get; init; }

    // 2 floats
    [JsonPropertyName("uv_data")] public float[]? UvData { get; init; }

    // Colors are assume to be 4 floats
    [JsonPropertyName("color_data")] public float[]? ColorData { get; init; }

    // 3 floats
    [JsonPropertyName("normal_data")] public float[]? NormalData { get; init; }

    // 1 
    [JsonPropertyName("weight_data")] public float[]? WeightData { get; init; }

    // uints
    [JsonPropertyName("index_data")] public uint[]? IndexData { get; init; }
    [JsonPropertyName("index_slots")] public IndicesPerMaterialData[]? IndexSlots { get; init; }

    public List<Vector3> GetVertexList() {
        if (VertexData.Length % 3 != 0)
        {
            return new List<Vector3>();
        }
            
        var vertexList = new List<Vector3>();
        for (int i = 0; i < VertexData.Length; i += 3)
        {
            vertexList.Add(new Vector3(VertexData[i], VertexData[i + 1], VertexData[i + 2]));
        }

        return vertexList;
    }


    // Drawing by index comes with performance penalty. Unless our target machine is memory limited
    // or we need the use of several index buffers to render different parts of our mesh. The index buffer will
    // only produce a penalty due the simple fact that requires a prefetch of those vertices. Where the simple method
    // DrawArrays benefits from data location.
    public void GetIndexArray(out uint[] indexArray)
    {
        if (IndexData is null) {
            indexArray = new uint[0];
        }
        else {
            indexArray = new uint[IndexData.Length];
            IndexData.CopyTo(indexArray, 0);
        }
    }

    // IMPORTANT: This always get sorted by VertexInfo enum definition.
    public void GetVertexDataPerVertex(out float[] vertexData, VertexInfo info)
    {
        if (info.HasFlag(VertexInfo.UV))
        {
            throw new Exception("Can not get vertex data per vertex when UV is active");
        }

        vertexData = new float[info.StrideSize() * (VertexData.Length / 3)];
        var offset = 0;
        
        if (info.HasFlag(VertexInfo.VERTICES))
        {
            fillData(ref vertexData, VertexData, offset, info.StrideSize(), VertexInfo.VERTICES.ComponentSize());
            offset += VertexInfo.VERTICES.ComponentSize();
        }
        if (info.HasFlag(VertexInfo.COLORS))
        {
            if (ColorData is null)
            {
                throw new Exception("Can not get vertex data per vertex when Color is not active");
            }
            fillData(ref vertexData, ColorData, offset, info.StrideSize(), VertexInfo.COLORS.ComponentSize());
            offset += VertexInfo.COLORS.ComponentSize();
        }
        if (info.HasFlag(VertexInfo.NORMALS))
        {
            if (NormalData is null)
            {
                throw new Exception("Can not get vertex data per vertex when Normal is not active");
            }
            fillData(ref vertexData, NormalData, offset, info.StrideSize(), VertexInfo.NORMALS.ComponentSize());
            offset += VertexInfo.NORMALS.ComponentSize();
        }
        if (info.HasFlag(VertexInfo.WEIGHTS))
        {
            if (WeightData is null)
            {
                throw new Exception("Can not get vertex data per vertex when Weight is not active");
            }
            fillData(ref vertexData, WeightData, offset, info.StrideSize(), VertexInfo.WEIGHTS.ComponentSize());
        }
    }

    public void fillData(ref float[] data, float[] subData, int offset, int stride, int componentSize)
    {
        for (int i = 0; i < subData.Length / componentSize; i++)
        {
            for (int j = 0; j < componentSize; j++)
            {
                data[offset + stride * i + j] = subData[i * componentSize + j];
            }
        }
    }

    // We only consider index data as triangle list. So, to get the number of triangles would be IndexData.Length / 3
    // Or the number of rows in the vertex data would be IndexData.Length
    // IMPORTANT: This always get sorted by VertexInfo enum definition.
    public void GetVertexDataPerTriangle(out float[] vertexData, VertexInfo info)
    {
        if (IndexData is null)
        {
            throw new Exception("Can not get vertex data per triangle when Index is not active");   
        }
        vertexData = new float[info.StrideSize() * IndexData.Length];
        var offset = 0;
        
        if (info.HasFlag(VertexInfo.VERTICES))
        {
            fillDataPerTriangle(ref vertexData, VertexData, offset, info.StrideSize(), VertexInfo.VERTICES.ComponentSize());
            offset += VertexInfo.VERTICES.ComponentSize();
        }
        
        if (info.HasFlag(VertexInfo.COLORS))
        {
            if (ColorData is null)
            {
                throw new Exception("Can not get vertex data per triangle when Color is not active");
            }
            fillDataPerTriangle(ref vertexData, ColorData, offset, info.StrideSize(), VertexInfo.COLORS.ComponentSize());
            offset += VertexInfo.COLORS.ComponentSize();
        }

        if (info.HasFlag(VertexInfo.UV))
        {
            if (UvData is null)
            {
                throw new Exception("Can not get vertex data per triangle when UV is not active");
            }
            for (int i = 0; i < UvData!.Length / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    vertexData[offset + info.StrideSize() * i + j] = UvData[i * 2 + j];
                }
            }
            offset += VertexInfo.UV.ComponentSize();
        }
        
        if (info.HasFlag(VertexInfo.NORMALS))
        {
            if (NormalData is null)
            {
                throw new Exception("Can not get vertex data per triangle when Normal is not active");
            }
            fillDataPerTriangle(ref vertexData, NormalData, offset, info.StrideSize(), VertexInfo.NORMALS.ComponentSize());
            offset += VertexInfo.NORMALS.ComponentSize();
        }
        
        if (info.HasFlag(VertexInfo.WEIGHTS))
        {
            if (WeightData is null)
            {
                throw new Exception("Can not get vertex data per triangle when Weight is not active");
            }
            fillDataPerTriangle(ref vertexData, WeightData, offset, info.StrideSize(), VertexInfo.WEIGHTS.ComponentSize());
        }
        
    }

    private void fillDataPerTriangle(ref float[] data, float[] subData, int offset, int stride, int componentSize)
    {
        for (int i = 0; i < IndexData!.Length; i++)
        {
            for (int j = 0; j < componentSize; j++)
            {
                data[offset + stride * i + j] = subData[IndexData[i] * componentSize + j];
            }
        }
    }

    public VertexInfo GetInfo()
    {
        var flags = VertexInfo.VERTICES;
        if (ColorData is not null)
            flags |= VertexInfo.COLORS;
        if (UvData is not null)
            flags |= VertexInfo.UV;
        if (NormalData is not null)
            flags |= VertexInfo.NORMALS;
        if (WeightData is not null)
            flags |= VertexInfo.WEIGHTS;

        return flags;
    }

    // TODO: validate size of each component
    public bool Validate()
    {
        return true;
    }
}

public record IndicesPerMaterialData
{
    [JsonPropertyName("start")] public uint Start { get; init; }
    [JsonPropertyName("offset")] public uint Offset { get; init; }
}

public record SceneJson
{
    // Scene ID
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("meshes")] public required MeshMetaData[] Meshes { get; init; }
    [JsonPropertyName("textures")] public required TextureData[] Textures { get; init; }
    
    [JsonPropertyName("materials")] public required Dictionary<string, SlotMaterialData[]> Materials { get; init; }
    [JsonPropertyName("actors")] public required ActorData[] Actors { get; init; }
}

public record MeshMetaData
{
    [JsonPropertyName("file")] public required string File { get; init; }
    [JsonPropertyName("id")] public required string Id { get; init; }
}

public record TextureData
{
    [JsonPropertyName("path")] public required string Path { get; init; }
    [JsonPropertyName("id")] public required string Id { get; init; }
}