using System.Text.Json.Serialization;
using Kalasrapier.Engine.Rendering;

namespace Kalasrapier.Engine.ImportJson;

public record ActorJson
{
    [JsonPropertyName("id")] public string Id { get; init; }
    [JsonPropertyName("render_pipeline")] public List<string> RenderPipeline { get; init; } = new();
    [JsonPropertyName("mesh_id")] public string? MeshId { get; init; }
    [JsonPropertyName("texture_id")] public string? TextureId { get; init; }
    [JsonPropertyName("enabled")] public bool Enabled { get; init; } = true;
    [JsonPropertyName("position")] public float[] Position { get; init; } = { 0, 0, 0 };
    [JsonPropertyName("scale")] public float[] Scale { get; init; } = { 1, 1, 1 };
    [JsonPropertyName("orientation")] public OrientationJson Orientation { get; init; } = new();
    [JsonPropertyName("children")] public List<ActorJson> Children { get; init; } = new();
    [JsonPropertyName("collider")] public ColliderType Collider { get; init; } = ColliderType.None;

    [JsonPropertyName("additional_attributes")]
    public Dictionary<string, object> AdditionalAttributes { get; init; } = new();
}

public record OrientationJson
{
    [JsonPropertyName("axis")] public float[] Axis { get; init; } = new float[] { 0, 1f, 0 };
    [JsonPropertyName("angle")] public float Angle { get; init; } = 0;
}

public record MeshJson
{
    // 3 floats
    [JsonPropertyName("vertex_data")] public float[] VertexData { get; init; }

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
    [JsonPropertyName("index_slots")] public IndicesPerMaterialJson[]? IndexSlots { get; init; }
    [JsonPropertyName("materials")] public MaterialJson[]? Materials { get; init; }
}

public record MaterialJson
{
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("diffuse_color")] public float[] DiffuseColor { get; init; }
}

public record IndicesPerMaterialJson
{
    [JsonPropertyName("start")] public uint Start { get; init; }
    [JsonPropertyName("offset")] public uint Offset { get; init; }
}

public record SceneJson
{
    // Scene ID
    [JsonPropertyName("id")] public string Id { get; init; }
    [JsonPropertyName("meshes")] public MeshMetaJson[] Meshes { get; init; }
    [JsonPropertyName("textures")] public TextureJson[] Textures { get; init; }
    [JsonPropertyName("actors")] public ActorJson[] Actors { get; init; }
}

public record MeshMetaJson
{
    [JsonPropertyName("file")] public string File { get; init; }
    [JsonPropertyName("id")] public string Id { get; init; }
}

public record TextureJson
{
    [JsonPropertyName("path")] public string Path { get; init; }
    [JsonPropertyName("id")] public string Id { get; init; }
}