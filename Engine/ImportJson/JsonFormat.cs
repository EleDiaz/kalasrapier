using System.Text.Json.Serialization;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Components;

namespace Kalasrapier.Engine.ImportJson;

public record ActorData
{
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("enabled")] public bool Enabled { get; init; } = true;
    [JsonPropertyName("position")] public float[] Position { get; init; } = { 0, 0, 0 };
    [JsonPropertyName("scale")] public float[] Scale { get; init; } = { 1, 1, 1 };
    [JsonPropertyName("orientation")] public OrientationData Orientation { get; init; } = new();
    [JsonPropertyName("children")] public List<ActorData> Children { get; init; } = new();
    [JsonPropertyName("components")] public List<ComponentData> Components { get; init; } = new();
    // [JsonPropertyName("render_pipeline")] public List<string> RenderPipeline { get; init; } = new();
    // [JsonPropertyName("mesh_id")] public string? MeshId { get; init; }
    // [JsonPropertyName("texture_id")] public string? TextureId { get; init; }
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
    [JsonPropertyName("materials")] public MaterialData[]? Materials { get; init; }
}

public record MaterialData
{
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("diffuse_color")] public required float[] DiffuseColor { get; init; }
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