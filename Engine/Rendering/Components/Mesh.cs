using Kalasrapier.Engine.Rendering.Components;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Components;

public class MeshData : ComponentData
{
    string MeshId { get; set; } = "";
}

public class Mesh : Component
{
    public List<Vector3> GetVertexList()
    {
        throw new NotImplementedException();
    }
}
