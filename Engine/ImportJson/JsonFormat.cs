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


    public void GetVertexArray(out float[] vertexData, VertexInfo info)
    {
        // We can't directly use the vertex length when we work with UVs. Due, that each UV is linked to a vertex
        // inside a triangle primitive and those primitive could have those vertices 0 or more times shared with others.
        // Also, one would be encouraged to think that the UV points would match the index points. But it is false
        // depending on the representation of those index points. We will be using one where the indexes aren't
        // reuse between triangles. So, our UVs will always be double size of our index buffer.
        //
        // The code is implemented to allow the case where the vertices, normals, colors... are associate to the
        // each polygon primitive.

        var getSize = (VertexInfo info) => {
            return UvData?.Length / 2 * info.ComponentSize();
        };

        var verticesLength = getSize(VertexInfo.VERTICES) ?? VertexData.Length;
        // Normals are associate to a vertex 1-1. This could change be to achieve a Flat Shading where each triangle
        // would share the face normal.
        var normalLength = getSize(VertexInfo.NORMALS) ?? NormalData?.Length ?? 0;
        // Colors are associate to a vertex 1-1.
        var colorsLength = getSize(VertexInfo.COLORS) ?? ColorData?.Length ?? 0;
        // Weights are associate to a vertex 1-1.
        var weightsLength = getSize(VertexInfo.COLORS) ?? WeightData?.Length ?? 0;

        var uvLength = UvData?.Length ?? 0;

        var size = verticesLength + normalLength + colorsLength + weightsLength + uvLength;

        var strideSize = info.StrideSize();
            
        var getOffset = (int currentOffset, VertexInfo component) => {
            return currentOffset + (info.HasFlag(component) ? component.ComponentSize() : 0);
        };

        vertexData = new float[size];
        int vI = 0;
        var vertexOffset = getOffset(0, VertexInfo.VERTICES);
        int cI = 0;
        var colorOffset = getOffset(vertexOffset, VertexInfo.COLORS);
        int uvI = 0;
        var uvOffset = getOffset(colorOffset, VertexInfo.UV);
        int nI = 0;
        var normalOffset = getOffset(uvOffset, VertexInfo.NORMALS);
        int wI = 0;
        var weightsOffset = getOffset(normalOffset, VertexInfo.WEIGHTS);


        var fillValuesWithIndices = (ref float[] vertexData, ref int ix, ref int index, float[] array, VertexInfo component) => {
            // When UV is active the index will be associate to Index the indices.
            if (info.HasFlag(VertexInfo.UV))
            {
                for (int j = 0; j < component.ComponentSize(); j++)
                {
                    vertexData[ix++] = array[3 * IndexData![index] + j];
                }
                ix--;
            }
            else
            {
                vertexData[ix] = array[index];
            }
            index++;
        };

        for (int i = 0; i < vertexData.Length; i++)
        {
            if (i % strideSize < vertexOffset)
            {
                fillValuesWithIndices(ref vertexData, ref i, ref vI, VertexData, VertexInfo.VERTICES);
            }
            else if (i % strideSize < colorOffset)
            {
                fillValuesWithIndices(ref vertexData, ref i, ref cI, ColorData!, VertexInfo.COLORS);
            }
            else if (i % strideSize < uvOffset)
            {
                vertexData[i] = UvData![uvI];
                uvI++;
            }
            else if (i % strideSize < normalOffset)
            {
                fillValuesWithIndices(ref vertexData, ref i, ref nI, NormalData!, VertexInfo.NORMALS);
            }
            else if (i % strideSize < weightsOffset)
            {
                fillValuesWithIndices(ref vertexData, ref i, ref wI, WeightData!, VertexInfo.WEIGHTS);
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
    
    [JsonPropertyName("materials")] public required MaterialData[] Materials { get; init; }
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